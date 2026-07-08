using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Security;

namespace WhatsPro.Http;

/// <summary>
/// Internal HTTP client wrapper for making requests to the Whats-Pro API.
/// </summary>
internal class WhatsProHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly WhatsProOptions _options;

    public WhatsProHttpClient(WhatsProOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = new HttpClient { BaseAddress = new Uri(_options.BaseUrl) };
        _httpClient.Timeout = _options.Timeout;
        
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest request, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(request, JsonOptions.Default);
        string encryptedPayload = PayloadEncryptor.Encrypt(json, _options.EncryptionKey);
        
        var payloadObj = new { payload = encryptedPayload };
        string payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions.Default);
        
        using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PostAsync(uri, content, cancellationToken).ConfigureAwait(false);
        
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest request, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(request, JsonOptions.Default);
        string encryptedPayload = PayloadEncryptor.Encrypt(json, _options.EncryptionKey);
        
        var payloadObj = new { payload = encryptedPayload };
        string payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions.Default);

        using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PutAsync(uri, content, cancellationToken).ConfigureAwait(false);

        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string uri, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync(uri, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    private async Task<TResponse> ProcessResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        response.EnsureSuccessStatusCode();

        string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        
#if NET48 || NETSTANDARD2_0
        if (string.IsNullOrEmpty(json))
            return default!;
#endif
        
        string decryptedJson;
        try 
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("payload", out var payloadElement))
            {
                string encryptedPayload = payloadElement.GetString() ?? string.Empty;
                decryptedJson = PayloadEncryptor.Decrypt(encryptedPayload, _options.EncryptionKey);
            }
            else
            {
                decryptedJson = json; // not encrypted
            }
        }
        catch (JsonException)
        {
            // It might be a plain encrypted string
            decryptedJson = PayloadEncryptor.Decrypt(json, _options.EncryptionKey);
        }

        if (string.IsNullOrEmpty(decryptedJson))
            return default!;

        return JsonSerializer.Deserialize<TResponse>(decryptedJson, JsonOptions.Default)!;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

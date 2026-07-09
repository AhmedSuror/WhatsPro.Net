using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Exceptions;
using WhatsPro.Internal;
using WhatsPro.Models;
using WhatsPro.Security;

namespace WhatsPro.Http;

/// <summary>
/// Internal HTTP client wrapper for making requests to the Whats-Pro API.
/// </summary>
internal class WhatsProHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly WhatsProOptions _options;
    private readonly AuthenticationManager _authManager;

    public WhatsProHttpClient(WhatsProOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = new HttpClient { BaseAddress = new Uri(_options.BaseUrl) };
        _httpClient.Timeout = _options.Timeout;
        
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _authManager = new AuthenticationManager(_options, this);
    }

    private async Task EnsureAuthenticationAsync(HttpRequestMessage requestMessage, bool skipAuth, CancellationToken cancellationToken)
    {
        if (!skipAuth)
        {
            string token = await _authManager.GetTokenAsync(cancellationToken).ConfigureAwait(false);
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);

        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest requestData, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(requestData, JsonOptions.Default);
        string encryptedPayload = PayloadEncryptor.Encrypt(json, _options.EncryptionKey);
        
        var payloadObj = new { payload = encryptedPayload };
        string payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions.Default);
        
        using var request = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);
        
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest requestData, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(requestData, JsonOptions.Default);
        string encryptedPayload = PayloadEncryptor.Encrypt(json, _options.EncryptionKey);
        
        var payloadObj = new { payload = encryptedPayload };
        string payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions.Default);

        using var request = new HttpRequestMessage(HttpMethod.Put, uri)
        {
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);

        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string uri, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, uri);
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);

        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PostMultipartAsync<TResponse>(string uri, string parameterName, string fileName, System.IO.Stream fileStream, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, uri);
        await EnsureAuthenticationAsync(request, skipAuth: false, cancellationToken).ConfigureAwait(false);
        
        var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        content.Add(streamContent, parameterName, fileName);
        request.Content = content;
        
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (HttpRequestException ex)
        {
            throw new NetworkException("Network error occurred while sending the request.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new NetworkException("The request timed out.", ex);
        }
    }

    private async Task<TResponse> ProcessResponseAsync<TResponse>(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.StatusCode == (System.Net.HttpStatusCode)422)
        {
            string errorJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            throw new ValidationException($"Validation failed (422): {errorJson}");
        }

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            throw new NetworkException($"HTTP request failed with status code {response.StatusCode}.", ex);
        }

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

        var result = JsonSerializer.Deserialize<TResponse>(decryptedJson, JsonOptions.Default)!;

        if (result is IWhatsProResponse apiResponse && !apiResponse.Success)
        {
            throw new ApiException(apiResponse.Message ?? "The API returned an unsuccessful response.");
        }

        return result;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

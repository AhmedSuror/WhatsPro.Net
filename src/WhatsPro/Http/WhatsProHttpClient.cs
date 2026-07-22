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
        _httpClient = new HttpClient();
        _httpClient.Timeout = _options.Timeout;
        
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        _authManager = new AuthenticationManager(_options, this);
    }

    internal WhatsProHttpClient(WhatsProOptions options, HttpMessageHandler messageHandler)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _httpClient = new HttpClient(messageHandler);
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

    private string BuildUrl(string uri)
    {
        string baseUrl = _options.BaseUrl.TrimEnd('/');
        string relativeUri = uri.TrimStart('/');
        return $"{baseUrl}/{relativeUri}";
    }

    public async Task<TResponse> GetAsync<TResponse>(string uri, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildUrl(uri));
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);

        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    internal Task<string> GetApiTokenAsync(CancellationToken cancellationToken)
    {
        return _authManager.GetApiTokenAsync(cancellationToken);
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(string uri, TRequest requestData, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(requestData, JsonOptions.Default);
        string encryptedPayload = PayloadEncryptor.Encrypt(json, _options.EncryptionKey);
        
        var payloadObj = new { payload = encryptedPayload };
        string payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions.Default);
        
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(uri))
        {
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);
        
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PostUnencryptedAsync<TRequest, TResponse>(string uri, TRequest requestData, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(requestData, JsonOptions.Default);
        
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(uri))
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        if (!skipAuth)
        {
            string apiToken = await _authManager.GetApiTokenAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(apiToken))
                request.Headers.Add("token", apiToken);
        }
        
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(string uri, TRequest requestData, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        string json = JsonSerializer.Serialize(requestData, JsonOptions.Default);
        string encryptedPayload = PayloadEncryptor.Encrypt(json, _options.EncryptionKey);
        
        var payloadObj = new { payload = encryptedPayload };
        string payloadJson = JsonSerializer.Serialize(payloadObj, JsonOptions.Default);

        using var request = new HttpRequestMessage(HttpMethod.Put, BuildUrl(uri))
        {
            Content = new StringContent(payloadJson, Encoding.UTF8, "application/json")
        };
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);

        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string uri, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, BuildUrl(uri));
        await EnsureAuthenticationAsync(request, skipAuth, cancellationToken).ConfigureAwait(false);

        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PostMultipartAsync<TResponse>(string uri, string parameterName, string fileName, System.IO.Stream fileStream, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(uri));
        await EnsureAuthenticationAsync(request, skipAuth: false, cancellationToken).ConfigureAwait(false);
        
        var content = new MultipartFormDataContent();
        var streamContent = new StreamContent(fileStream);
        content.Add(streamContent, parameterName, fileName);
        request.Content = content;
        
        using var response = await SendRequestAsync(request, cancellationToken).ConfigureAwait(false);
        return await ProcessResponseAsync<TResponse>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task<TResponse> PostMultipartUnencryptedAsync<TResponse>(string uri, string parameterName, string fileName, System.IO.Stream fileStream, bool skipAuth = false, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildUrl(uri));
        
        if (!skipAuth)
        {
            string apiToken = await _authManager.GetApiTokenAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrEmpty(apiToken))
                request.Headers.Add("token", apiToken);
        }

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
        string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (response.StatusCode == (System.Net.HttpStatusCode)422)
        {
            string errorMessage = $"Validation failed (422): {json}";
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Object)
                {
                    var errorMessages = new System.Collections.Generic.List<string>();
                    foreach (var property in errorsProp.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var error in property.Value.EnumerateArray())
                            {
                                string errStr = error.GetString() ?? string.Empty;
                                if (!string.IsNullOrEmpty(errStr))
                                    errorMessages.Add(errStr);
                            }
                        }
                    }
                    if (errorMessages.Count > 0)
                    {
                        errorMessage = string.Join(" ", errorMessages);
                    }
                }
                else if (doc.RootElement.TryGetProperty("message", out var msgProp))
                {
                    errorMessage = msgProp.GetString() ?? errorMessage;
                }
            }
            catch { }

            throw new ValidationException(errorMessage);
        }

        string decryptedJson = string.Empty;
        if (!string.IsNullOrEmpty(json))
        {
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
                try
                {
                    decryptedJson = PayloadEncryptor.Decrypt(json, _options.EncryptionKey);
                }
                catch
                {
                    decryptedJson = json;
                }
            }
        }

        TResponse result = default!;
        if (!string.IsNullOrEmpty(decryptedJson))
        {
            try
            {
                result = JsonSerializer.Deserialize<TResponse>(decryptedJson, JsonOptions.Default)!;
            }
            catch { }
        }

        if (result is IWhatsProResponse apiResponse && !apiResponse.Success)
        {
            throw new ApiException(apiResponse.Message ?? "The API returned an unsuccessful response.");
        }

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            string details = string.IsNullOrEmpty(decryptedJson) ? json : decryptedJson;
            throw new NetworkException($"HTTP request failed with status code {response.StatusCode}. Details: {details}", ex);
        }

        if (result == null)
            return default!;

        return result;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}

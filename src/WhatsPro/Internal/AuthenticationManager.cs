using System;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Authentication.Models;
using WhatsPro.Exceptions;
using WhatsPro.Http;

namespace WhatsPro.Internal;

internal class AuthenticationManager
{
    private readonly WhatsProOptions _options;
    private readonly WhatsProHttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    private string? _accessToken;
    private string? _apiToken;
    private DateTime _expiresAt;

    public AuthenticationManager(WhatsProOptions options, WhatsProHttpClient httpClient)
    {
        _options = options;
        _httpClient = httpClient;
    }

    public async Task<string> GetTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt)
        {
            return _accessToken!;
        }

        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < _expiresAt)
            {
                return _accessToken!;
            }

            var request = new LoginRequest { Email = _options.Email, Password = _options.Password };
            
            // Pass skipAuth: true to prevent infinite loop during login.
            // LoginResponse is a flat model — the login endpoint does not wrap
            // its payload inside a "data" object like other endpoints do.
            var response = await _httpClient.PostAsync<LoginRequest, LoginResponse>(
                "/user/login", 
                request, 
                skipAuth: true, 
                cancellationToken).ConfigureAwait(false);

            if (response == null || !response.Success)
            {
                throw new AuthenticationException(response?.Message ?? "Failed to authenticate.");
            }

            _accessToken = response.AccessToken;
            _apiToken = response.User?.ApiToken;

            // The API says token expires in 168 hours. Let's subtract 5 minutes for safety.
            _expiresAt = DateTime.UtcNow.AddHours(response.ExpiresInHours).AddMinutes(-5);
            
            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<string> GetApiTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(_options.ApiToken))
            return _options.ApiToken;

        // Ensure we are logged in so we can fetch it
        await GetTokenAsync(cancellationToken).ConfigureAwait(false);
        return _apiToken ?? string.Empty;
    }
}

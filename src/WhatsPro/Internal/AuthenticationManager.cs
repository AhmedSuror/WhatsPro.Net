using System;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Authentication.Models;
using WhatsPro.Exceptions;
using WhatsPro.Http;
using WhatsPro.Models;

namespace WhatsPro.Internal;

internal class AuthenticationManager
{
    private readonly WhatsProOptions _options;
    private readonly WhatsProHttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    private string? _accessToken;
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
            
            // Pass skipAuth: true to prevent infinite loop during login
            var response = await _httpClient.PostAsync<LoginRequest, WhatsProResponse<LoginResponse>>(
                "/user/login", 
                request, 
                skipAuth: true, 
                cancellationToken).ConfigureAwait(false);

            if (response == null || !response.Success || response.Data == null)
            {
                throw new AuthenticationException(response?.Message ?? "Failed to authenticate.");
            }

            _accessToken = response.Data.AccessToken;
            
            // The API says token expires in 168 hours. Let's subtract 5 minutes for safety.
            _expiresAt = DateTime.UtcNow.AddHours(response.Data.ExpiresInHours).AddMinutes(-5);
            
            return _accessToken;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

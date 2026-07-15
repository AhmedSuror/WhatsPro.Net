using System;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Authentication.Models;
using WhatsPro.Http;
using WhatsPro.Models;

namespace WhatsPro.Authentication;

/// <summary>
/// Operations related to user authentication and profile management.
/// </summary>
public class AuthOperations
{
    private readonly WhatsProHttpClient _httpClient;

    internal AuthOperations(WhatsProHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Logs in the user and returns an access token.
    /// </summary>
    public async Task<WhatsProResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<LoginRequest, WhatsProResponse<LoginResponse>>("/user/login", request, skipAuth: true, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    public async Task<WhatsProResponse<LoginResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<RegisterRequest, WhatsProResponse<LoginResponse>>("/user/register", request, skipAuth: true, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the current user's profile information.
    /// </summary>
    public async Task<WhatsProResponse<UserInfo>> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<UserInfo>>("/user/profile", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates the current user's profile information.
    /// </summary>
    public async Task<WhatsProResponse<string>> UpdateProfileAsync(UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<UpdateProfileRequest, WhatsProResponse<string>>("/user/profile", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Changes the current user's password.
    /// </summary>
    public async Task<WhatsProResponse<string>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<ChangePasswordRequest, WhatsProResponse<string>>("/user/profile/password", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the current API token used for non-encrypted endpoints.
    /// </summary>
    public async Task<string> GetApiTokenAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetApiTokenAsync(cancellationToken).ConfigureAwait(false);
    }
}

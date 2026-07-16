using System;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Http;
using WhatsPro.Models;
using WhatsPro.Sessions.Models;

namespace WhatsPro.Sessions;

/// <summary>
/// Operations related to session management.
/// </summary>
public class SessionOperations
{
    private readonly WhatsProHttpClient _httpClient;

    internal SessionOperations(WhatsProHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Retrieves a paginated list of sessions.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated response containing session information.</returns>
    public async Task<WhatsProResponse<PagedResponse<SessionInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<SessionInfo>>>("/sessions/index", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Attempts to connect to a specific WhatsApp session (gets QR code or connects if already authenticated).
    /// </summary>
    /// <param name="id">The unique identifier of the session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session information including QR data if needed.</returns>
    public async Task<WhatsProResponse<SessionInfo>> ConnectAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<SessionInfo>>($"/sessions/connect/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Disconnects an active WhatsApp session.
    /// </summary>
    /// <param name="id">The unique identifier of the session.</param>
    /// <param name="forever">True to permanently disconnect (delete credentials), false to just stop the session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating success or failure of the disconnect operation.</returns>
    public async Task<WhatsProResponse<string>> DisconnectAsync(int id, bool forever, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<string>>($"/sessions/disconnect/{id}?forever={forever.ToString().ToLower()}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves detailed information about a specific session.
    /// </summary>
    /// <param name="id">The unique identifier of the session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed session information.</returns>
    public async Task<WhatsProResponse<SessionInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<SessionInfo>>($"/sessions/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Triggers a change name operation for a session.
    /// </summary>
    /// <param name="id">The unique identifier of the session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating success or failure of the operation.</returns>
    public async Task<WhatsProResponse<string>> ChangeNameAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<string>>($"/sessions/change_name/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Configures the webhook URL for a specific session to receive incoming events.
    /// </summary>
    /// <param name="id">The unique identifier of the session.</param>
    /// <param name="request">The webhook configuration details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating success or failure of updating the webhook.</returns>
    public async Task<WhatsProResponse<string>> SetWebhookAsync(int id, SetWebhookRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<SetWebhookRequest, WhatsProResponse<string>>($"/sessions/webhook/{id}", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }
}

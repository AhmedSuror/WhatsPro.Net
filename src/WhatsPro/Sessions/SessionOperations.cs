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

    public async Task<WhatsProResponse<PagedResponse<SessionInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<SessionInfo>>>("/sessions/index", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<SessionInfo>> ConnectAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<SessionInfo>>($"/sessions/connect/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> DisconnectAsync(int id, bool forever, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<string>>($"/sessions/disconnect/{id}?forever={forever.ToString().ToLower()}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<SessionInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<SessionInfo>>($"/sessions/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> ChangeNameAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<string>>($"/sessions/change_name/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> SetWebhookAsync(int id, SetWebhookRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<SetWebhookRequest, WhatsProResponse<string>>($"/sessions/webhook/{id}", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }
}

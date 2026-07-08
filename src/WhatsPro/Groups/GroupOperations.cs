using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Groups.Models;
using WhatsPro.Http;
using WhatsPro.Models;

namespace WhatsPro.Groups;

/// <summary>
/// Operations related to groups management.
/// </summary>
public class GroupOperations
{
    private readonly WhatsProHttpClient _httpClient;

    internal GroupOperations(WhatsProHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<WhatsProResponse<PagedResponse<GroupInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<GroupInfo>>>("/groups/list", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<CreateGroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<CreateGroupRequest, WhatsProResponse<CreateGroupResponse>>("/groups/create", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<GroupInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<GroupInfo>>($"/groups/get/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<List<GroupInfo>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<List<GroupInfo>>>("/groups/all", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> UpdateAsync(int id, UpdateGroupRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<UpdateGroupRequest, WhatsProResponse<string>>($"/groups/update/{id}", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/groups/delete", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> TransferClientsAsync(TransferClientsRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<TransferClientsRequest, WhatsProResponse<string>>("/groups/transfer", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> DeleteClientsAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/groups/delete_clients", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }
}

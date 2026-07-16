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

    /// <summary>
    /// Retrieves a paginated list of groups.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated response containing group information.</returns>
    public async Task<WhatsProResponse<PagedResponse<GroupInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<GroupInfo>>>("/groups/list", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new group.
    /// </summary>
    /// <param name="request">The group creation details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response containing the ID of the newly created group.</returns>
    public async Task<WhatsProResponse<CreateGroupResponse>> CreateAsync(CreateGroupRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<CreateGroupRequest, WhatsProResponse<CreateGroupResponse>>("/groups/create", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a group's details by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the group.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed information about the requested group.</returns>
    public async Task<WhatsProResponse<GroupInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<GroupInfo>>($"/groups/get/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a comprehensive list of all groups without pagination.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list containing all groups.</returns>
    public async Task<WhatsProResponse<List<GroupInfo>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<List<GroupInfo>>>("/groups/all", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing group's information.
    /// </summary>
    /// <param name="id">The ID of the group to update.</param>
    /// <param name="request">The updated group details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating success or failure of the update operation.</returns>
    public async Task<WhatsProResponse<string>> UpdateAsync(int id, UpdateGroupRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<UpdateGroupRequest, WhatsProResponse<string>>($"/groups/update/{id}", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes one or more groups.
    /// </summary>
    /// <param name="request">The request containing the IDs of the groups to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating the result of the deletion.</returns>
    public async Task<WhatsProResponse<string>> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/groups/delete", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Transfers clients from one group to another.
    /// </summary>
    /// <param name="request">The request detailing the source and destination groups, and optionally which clients to transfer.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating the success of the transfer operation.</returns>
    public async Task<WhatsProResponse<string>> TransferClientsAsync(TransferClientsRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<TransferClientsRequest, WhatsProResponse<string>>("/groups/transfer", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes specific clients from a group.
    /// </summary>
    /// <param name="request">The request containing the client IDs to delete from their respective groups.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating the success of the client deletion operation.</returns>
    public async Task<WhatsProResponse<string>> DeleteClientsAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/groups/delete_clients", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }
}

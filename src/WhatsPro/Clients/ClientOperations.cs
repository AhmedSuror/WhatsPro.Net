using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Clients.Models;
using WhatsPro.Http;
using WhatsPro.Models;

namespace WhatsPro.Clients;

/// <summary>
/// Operations related to clients management.
/// </summary>
public class ClientOperations
{
    private readonly WhatsProHttpClient _httpClient;

    internal ClientOperations(WhatsProHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Retrieves a paginated list of clients.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated response containing client information.</returns>
    public async Task<WhatsProResponse<PagedResponse<ClientInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<ClientInfo>>>("/clients/list", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates a new client.
    /// </summary>
    /// <param name="request">The client creation details (e.g., name, group id).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response containing the ID of the newly created client.</returns>
    public async Task<WhatsProResponse<CreateClientResponse>> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<CreateClientRequest, WhatsProResponse<CreateClientResponse>>("/clients/create", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves a client's details by their ID.
    /// </summary>
    /// <param name="id">The unique identifier of the client.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed information about the requested client.</returns>
    public async Task<WhatsProResponse<ClientInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<ClientInfo>>($"/clients/get/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Updates an existing client's information.
    /// </summary>
    /// <param name="id">The ID of the client to update.</param>
    /// <param name="request">The updated client details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating success or failure of the update operation.</returns>
    public async Task<WhatsProResponse<string>> UpdateAsync(int id, UpdateClientRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<UpdateClientRequest, WhatsProResponse<string>>($"/clients/update/{id}", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes one or more clients.
    /// </summary>
    /// <param name="request">The request containing the IDs of the clients to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating the result of the deletion.</returns>
    public async Task<WhatsProResponse<string>> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/clients/delete", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds one or more phone numbers to an existing client.
    /// </summary>
    /// <param name="request">The request containing the client ID and the list of phones to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating the success of adding phone numbers.</returns>
    public async Task<WhatsProResponse<string>> AddPhoneAsync(AddPhoneRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<AddPhoneRequest, WhatsProResponse<string>>("/clients/phones/add", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Moves a client from their current group to another group.
    /// </summary>
    /// <param name="request">The request containing the client ID and the target group ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating success or failure.</returns>
    public async Task<WhatsProResponse<string>> ChangeGroupAsync(ChangeGroupRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<ChangeGroupRequest, WhatsProResponse<string>>("/clients/change_group", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Imports clients in bulk from an Excel file.
    /// </summary>
    /// <param name="fileStream">The stream of the Excel file to import.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating the status of the import operation.</returns>
    public async Task<WhatsProResponse<string>> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostMultipartAsync<WhatsProResponse<string>>("/clients/import_from_excel", "client_data", "import.xlsx", fileStream, cancellationToken).ConfigureAwait(false);
    }
}

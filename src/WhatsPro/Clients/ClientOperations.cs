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

    public async Task<WhatsProResponse<PagedResponse<ClientInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<ClientInfo>>>("/clients/list", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<CreateClientResponse>> CreateAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<CreateClientRequest, WhatsProResponse<CreateClientResponse>>("/clients/create", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<ClientInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<ClientInfo>>($"/clients/get/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> UpdateAsync(int id, UpdateClientRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<UpdateClientRequest, WhatsProResponse<string>>($"/clients/update/{id}", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/clients/delete", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> AddPhoneAsync(AddPhoneRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<AddPhoneRequest, WhatsProResponse<string>>("/clients/phones/add", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> ChangeGroupAsync(ChangeGroupRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<ChangeGroupRequest, WhatsProResponse<string>>("/clients/change_group", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> ImportFromExcelAsync(Stream fileStream, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostMultipartAsync<WhatsProResponse<string>>("/clients/import_from_excel", "client_data", "import.xlsx", fileStream, cancellationToken).ConfigureAwait(false);
    }
}

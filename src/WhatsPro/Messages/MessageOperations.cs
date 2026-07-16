using System;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Http;
using WhatsPro.Messages.Models;
using WhatsPro.Models;

namespace WhatsPro.Messages;

/// <summary>
/// Operations related to message sending and management.
/// </summary>
public class MessageOperations
{
    private readonly WhatsProHttpClient _httpClient;

    internal MessageOperations(WhatsProHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<WhatsProResponse<PagedResponse<MessageInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<MessageInfo>>>("/user/messages/index", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<MessageInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<MessageInfo>>($"/user/messages/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/user/messages/delete", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> SendAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<SendMessageRequest, WhatsProResponse<string>>("/user/messages/send", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<string>> SendNonEncryptedAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostUnencryptedAsync<SendMessageRequest, WhatsProResponse<string>>("/messages/send", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WhatsProResponse<DocumentUploadResponse>> UploadDocumentAsync(string fileName, System.IO.Stream fileStream, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty.", nameof(fileName));
        if (fileStream == null)
            throw new ArgumentNullException(nameof(fileStream));

        return await _httpClient.PostMultipartUnencryptedAsync<WhatsProResponse<DocumentUploadResponse>>(
            "/documents/token-upload", 
            "file", 
            fileName, 
            fileStream, 
            skipAuth: false, 
            cancellationToken).ConfigureAwait(false);
    }
}

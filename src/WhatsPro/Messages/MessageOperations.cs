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

    /// <summary>
    /// Retrieves a paginated list of messages sent or received.
    /// </summary>
    /// <param name="request">The pagination and filtering parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paginated response containing message information.</returns>
    public async Task<WhatsProResponse<PagedResponse<MessageInfo>>> ListAsync(PaginationRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<PaginationRequest, WhatsProResponse<PagedResponse<MessageInfo>>>("/user/messages/index", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Retrieves detailed information about a specific message.
    /// </summary>
    /// <param name="id">The unique identifier of the message.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Detailed information about the requested message.</returns>
    public async Task<WhatsProResponse<MessageInfo>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<MessageInfo>>($"/user/messages/{id}", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Deletes one or more messages.
    /// </summary>
    /// <param name="request">The request containing the IDs of the messages to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating the success of the deletion operation.</returns>
    public async Task<WhatsProResponse<string>> DeleteAsync(DeleteRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<DeleteRequest, WhatsProResponse<string>>("/user/messages/delete", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a message using the secure encrypted endpoint. This uses an encrypted payload and requires valid session and AES keys.
    /// </summary>
    /// <param name="request">The details of the message to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating whether the message was queued successfully.</returns>
    public async Task<WhatsProResponse<string>> SendAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<SendMessageRequest, WhatsProResponse<string>>("/user/messages/send", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a message using the unencrypted fallback endpoint. Useful for simple integrations, uses static API token under the hood.
    /// </summary>
    /// <param name="request">The details of the message to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response indicating whether the message was sent successfully.</returns>
    public async Task<WhatsProResponse<string>> SendNonEncryptedAsync(SendMessageRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostUnencryptedAsync<SendMessageRequest, WhatsProResponse<string>>("/messages/send", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Uploads a document to the server for sending in messages via the unencrypted token endpoint.
    /// </summary>
    /// <param name="fileName">The name of the file to be uploaded.</param>
    /// <param name="fileStream">The stream containing the file data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A response containing the URL or identifier of the uploaded document.</returns>
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

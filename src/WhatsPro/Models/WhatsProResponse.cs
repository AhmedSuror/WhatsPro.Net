using System.Text.Json.Serialization;

namespace WhatsPro.Models;

/// <summary>
/// Defines the standard properties of a Whats-Pro API response.
/// </summary>
public interface IWhatsProResponse
{
    bool Success { get; }
    string Message { get; }
}

/// <summary>
/// A generic response wrapper for the Whats-Pro API.
/// </summary>
/// <typeparam name="T">The type of the data.</typeparam>
public class WhatsProResponse<T> : IWhatsProResponse
{
    /// <summary>
    /// Indicates whether the request was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// A message from the server, typically containing error details or success confirmation.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The response payload.
    /// </summary>
    public T Data { get; set; } = default!;
}

using System;

namespace WhatsPro;

/// <summary>
/// Configuration options for the WhatsProClient.
/// </summary>
public class WhatsProOptions
{
    /// <summary>
    /// The base URL of the Whats-Pro API.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The email address used for authentication.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The password used for authentication.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// The key used for payload encryption and decryption. Defaults to the API standard.
    /// </summary>
    public string EncryptionKey { get; set; } = "abcd123456789ABCD";

    /// <summary>
    /// The timeout for API requests. Defaults to 100 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);
}

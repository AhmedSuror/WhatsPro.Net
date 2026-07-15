using System.Text.Json.Serialization;

namespace WhatsPro.Messages.Models;

public class DocumentUploadResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("original_name")]
    public string OriginalName { get; set; } = string.Empty;
    
    [JsonPropertyName("mime_type")]
    public string MimeType { get; set; } = string.Empty;
    
    [JsonPropertyName("size")]
    public long Size { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsPro.Messages.Models;

public class MessageInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("from")]
    public string From { get; set; } = string.Empty;

    [JsonPropertyName("to")]
    public string To { get; set; } = string.Empty;

    [JsonPropertyName("img")]
    public string? Img { get; set; }

    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }

    [JsonPropertyName("doc_path")]
    public string? DocPath { get; set; }

    [JsonPropertyName("doc_name")]
    public string? DocName { get; set; }

    [JsonPropertyName("doc_mime")]
    public string? DocMime { get; set; }

    [JsonPropertyName("has_document")]
    public bool HasDocument { get; set; }

    [JsonPropertyName("document_name")]
    public string? DocumentName { get; set; }

    [JsonPropertyName("document_mime")]
    public string? DocumentMime { get; set; }

    [JsonPropertyName("channel")]
    public string Channel { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("message_id")]
    public string MessageId { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class SendMessageRequest
{
    [JsonPropertyName("send_phone")]
    public bool SendPhone { get; set; }
    [JsonPropertyName("phones")]
    public List<string> Phones { get; set; } = new List<string>();
    [JsonPropertyName("send_group")]
    public bool SendGroup { get; set; }
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }
    [JsonPropertyName("send_client")]
    public bool SendClient { get; set; }
    [JsonPropertyName("client_ids")]
    public List<int> ClientIds { get; set; } = new List<int>();
    [JsonPropertyName("img")]
    public string Img { get; set; } = string.Empty;
    [JsonPropertyName("client_default_phone")]
    public bool ClientDefaultPhone { get; set; }
    [JsonPropertyName("send_all_clients")]
    public bool SendAllClients { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("country_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CountryCode { get; set; }

    [JsonPropertyName("img_url")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ImgUrl { get; set; }

    [JsonPropertyName("doc_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? DocId { get; set; }
}

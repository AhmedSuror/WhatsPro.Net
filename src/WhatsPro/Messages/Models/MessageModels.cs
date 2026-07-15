using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsPro.Messages.Models;

public class MessageInfo
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public string Img { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
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

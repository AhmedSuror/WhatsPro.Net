using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WhatsPro.Groups.Models;

namespace WhatsPro.Clients.Models;

public class ClientInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("phones_count")]
    public int PhonesCount { get; set; }
    public string Phone { get; set; } = string.Empty;
    public GroupInfo? Group { get; set; }
    public List<PhoneInfo> Phones { get; set; } = new List<PhoneInfo>();
}

public class CreateClientRequest
{
    public string Phone { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }
    public string? Notes { get; set; }
}

public class CreateClientResponse
{
    public string Message { get; set; } = string.Empty;
    public int Id { get; set; }
}

public class UpdateClientRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }
    public string Phone { get; set; } = string.Empty;
}

public class ChangeGroupRequest
{
    public List<int> Ids { get; set; } = new List<int>();
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }
}

public class PhoneInfo
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool Default { get; set; }
    [JsonPropertyName("client_id")]
    public int ClientId { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class AddPhoneRequest
{
    public string Phone { get; set; } = string.Empty;
    [JsonPropertyName("client_id")]
    public int ClientId { get; set; }
}

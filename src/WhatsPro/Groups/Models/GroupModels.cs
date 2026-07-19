using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WhatsPro.Groups.Models;

public class GroupInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("clients_count")]
    public int ClientsCount { get; set; }
}

public class CreateGroupRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class CreateGroupResponse
{
    public string Message { get; set; } = string.Empty;
    public int Id { get; set; }
}

public class UpdateGroupRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class TransferClientsRequest
{
    public List<int> Ids { get; set; } = new List<int>();
    [JsonPropertyName("group_id")]
    public int GroupId { get; set; }
}

public class DeleteGroupClientsRequest
{
    public List<int> Ids { get; set; } = new List<int>();
}

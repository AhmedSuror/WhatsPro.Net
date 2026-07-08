using System;
using System.Collections.Generic;

namespace WhatsPro.Groups.Models;

public class GroupInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
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
    public int GroupId { get; set; }
}

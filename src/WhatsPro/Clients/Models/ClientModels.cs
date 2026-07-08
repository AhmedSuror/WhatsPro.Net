using System;
using System.Collections.Generic;
using WhatsPro.Groups.Models;

namespace WhatsPro.Clients.Models;

public class ClientInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int GroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int PhonesCount { get; set; }
    public string Phone { get; set; } = string.Empty;
    public GroupInfo? Group { get; set; }
    public List<PhoneInfo> Phones { get; set; } = new List<PhoneInfo>();
}

public class CreateClientRequest
{
    public string Phone { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
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
    public int GroupId { get; set; }
    public string Phone { get; set; } = string.Empty;
}

public class ChangeGroupRequest
{
    public List<int> Ids { get; set; } = new List<int>();
    public int GroupId { get; set; }
}

public class PhoneInfo
{
    public int Id { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool Default { get; set; }
    public int ClientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class AddPhoneRequest
{
    public string Phone { get; set; } = string.Empty;
    public int ClientId { get; set; }
}

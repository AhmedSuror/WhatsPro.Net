using System;

namespace WhatsPro.Sessions.Models;

public class SessionInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Qr { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string DisplayedName { get; set; } = string.Empty;
    public string ProfileImage { get; set; } = string.Empty;
    public string WebhookUrl { get; set; } = string.Empty;
    public string Jid { get; set; } = string.Empty;
}

public class SetWebhookRequest
{
    public string Url { get; set; } = string.Empty;
}



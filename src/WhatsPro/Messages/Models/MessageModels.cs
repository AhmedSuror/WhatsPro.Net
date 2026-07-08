using System;
using System.Collections.Generic;

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
    public string SendPhone { get; set; } = string.Empty;
    public List<string> Phones { get; set; } = new List<string>();
    public bool SendGroup { get; set; }
    public int GroupId { get; set; }
    public bool SendClient { get; set; }
    public List<int> ClientIds { get; set; } = new List<int>();
    public string Img { get; set; } = string.Empty;
    public bool ClientDefaultPhone { get; set; }
    public bool SendAllClients { get; set; }
    public string Message { get; set; } = string.Empty;
}

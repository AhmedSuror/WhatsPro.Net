namespace WhatsPro.Models;

/// <summary>
/// Represents a request for paginated data.
/// </summary>
public class PaginationRequest
{
    public int Count { get; set; } = 15;
    public int Page { get; set; } = 1;
    public string Search { get; set; } = string.Empty;
    public string OrderBy { get; set; } = "id";
    public string OrderDir { get; set; } = "desc";
    public int? Group { get; set; }
}

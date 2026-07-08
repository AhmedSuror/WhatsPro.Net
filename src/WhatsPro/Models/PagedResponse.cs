using System.Collections.Generic;

namespace WhatsPro.Models;

/// <summary>
/// Represents a paginated response from the Whats-Pro API.
/// </summary>
/// <typeparam name="T">The type of the items in the response.</typeparam>
public class PagedResponse<T>
{
    public int CurrentPage { get; set; }
    public List<T> Data { get; set; } = new List<T>();
    public string FirstPageUrl { get; set; } = string.Empty;
    public int? From { get; set; }
    public int LastPage { get; set; }
    public string LastPageUrl { get; set; } = string.Empty;
    public string NextPageUrl { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int PerPage { get; set; }
    public string PrevPageUrl { get; set; } = string.Empty;
    public int? To { get; set; }
    public int Total { get; set; }
}

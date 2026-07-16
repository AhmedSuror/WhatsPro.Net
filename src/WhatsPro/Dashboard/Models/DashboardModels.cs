using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhatsPro.Dashboard.Models;

public class DashboardResponse
{
    public List<DashboardCard> Cards { get; set; } = new List<DashboardCard>();
    public List<DashboardPercentage> Percentage { get; set; } = new List<DashboardPercentage>();
}

public class DashboardCard
{
    public string Name { get; set; } = string.Empty;
    public JsonElement? Value { get; set; }
    public string Icon { get; set; } = string.Empty;
}

public class DashboardPercentage
{
    public string Name { get; set; } = string.Empty;
    public PercentageData Data { get; set; } = new PercentageData();
    [JsonPropertyName("more_is_better")]
    public bool MoreIsBetter { get; set; }
    public string Details { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class PercentageData
{
    public decimal Current { get; set; }
    public decimal Limit { get; set; }
    public decimal Percentage { get; set; }
}

public class SendChartRequest
{
    public string Type { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Month { get; set; }
}

public class TopNumberItem
{
    public string Name { get; set; } = string.Empty;
    public int Value { get; set; }
    
    [JsonPropertyName("img_url")]
    public string? ImgUrl { get; set; }
    
    [JsonPropertyName("has_document")]
    public bool HasDocument { get; set; }
    
    [JsonPropertyName("document_name")]
    public string? DocumentName { get; set; }
    
    [JsonPropertyName("document_mime")]
    public string? DocumentMime { get; set; }
}

using System.Collections.Generic;
using System.Text.Json;

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

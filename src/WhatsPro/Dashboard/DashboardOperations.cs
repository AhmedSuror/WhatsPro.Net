using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WhatsPro.Dashboard.Models;
using WhatsPro.Http;
using WhatsPro.Models;

namespace WhatsPro.Dashboard;

/// <summary>
/// Operations related to the dashboard and analytics.
/// </summary>
public class DashboardOperations
{
    private readonly WhatsProHttpClient _httpClient;

    internal DashboardOperations(WhatsProHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Gets the dashboard statistics.
    /// </summary>
    public async Task<WhatsProResponse<DashboardResponse>> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<WhatsProResponse<DashboardResponse>>("/dashboard/dashboard", skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the send chart statistics.
    /// </summary>
    public async Task<WhatsProResponse<List<int>>> GetSendChartAsync(SendChartRequest request, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<SendChartRequest, WhatsProResponse<List<int>>>("/dashboard/send_chart", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the top numbers statistics.
    /// </summary>
    public async Task<WhatsProResponse<List<object>>> GetTopNumbersAsync(int top, CancellationToken cancellationToken = default)
    {
        var request = new { Top = top };
        return await _httpClient.PostAsync<object, WhatsProResponse<List<object>>>("/dashboard/top_numbers", request, skipAuth: false, cancellationToken).ConfigureAwait(false);
    }
}

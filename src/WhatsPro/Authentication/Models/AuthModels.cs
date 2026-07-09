using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using WhatsPro.Models;

namespace WhatsPro.Authentication.Models;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string RePassword { get; set; } = string.Empty;
}

/// <summary>
/// Flat response returned by the login endpoint.
/// The API does not wrap login data inside a <c>data</c> object, so this
/// model implements <see cref="IWhatsProResponse"/> directly.
/// </summary>
public class LoginResponse : IWhatsProResponse
{
    /// <inheritdoc/>
    public bool Success { get; set; }

    /// <inheritdoc/>
    public string Message { get; set; } = string.Empty;

    /// <summary>Bearer token to use in subsequent requests.</summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>Token type (e.g. "bearer").</summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    /// <summary>Number of hours until the token expires.</summary>
    [JsonPropertyName("expires_in_hours")]
    public int ExpiresInHours { get; set; }

    /// <summary>Authenticated user details.</summary>
    public UserInfo? User { get; set; }
}

public class UserInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    [JsonPropertyName("display_name")]
    public string? DisplayName { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    [JsonPropertyName("country")]
    public string? Country { get; set; }
    [JsonPropertyName("bio")]
    public string? Bio { get; set; }
    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
    [JsonPropertyName("two_factor_enabled_at")]
    public DateTime? TwoFactorEnabledAt { get; set; }
    [JsonPropertyName("paused_at")]
    public DateTime? PausedAt { get; set; }
    [JsonPropertyName("webhook_url")]
    public string? WebhookUrl { get; set; }
    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }
    [JsonPropertyName("expires_at")]
    public DateTime? ExpiresAt { get; set; }
    [JsonPropertyName("api_token")]
    public string? ApiToken { get; set; }
    [JsonPropertyName("profile_id")]
    public int ProfileId { get; set; }
    [JsonPropertyName("is_admin")]
    public bool IsAdmin { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    public int Messages { get; set; }
    [JsonPropertyName("points_usage_daily")]
    public int PointsUsageDaily { get; set; }
    [JsonPropertyName("points_usage_monthly")]
    public int PointsUsageMonthly { get; set; }
    [JsonPropertyName("pay_as_you_go")]
    public int PayAsYouGo { get; set; }
    public int Verified { get; set; }
    [JsonPropertyName("in_egypt")]
    public int InEgypt { get; set; }
    [JsonPropertyName("max_sessions")]
    public int MaxSessions { get; set; }
    public bool Expired { get; set; }
    [JsonPropertyName("used_points")]
    public int UsedPoints { get; set; }
    /// <summary>The subscription plan assigned to this user. Populated by the profile endpoint.</summary>
    public ProfileInfo? Profile { get; set; }
}


public class ProfileInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public int Days { get; set; }
    public string Type { get; set; } = string.Empty;
    [JsonPropertyName("daily_points")]
    public int DailyPoints { get; set; }
    [JsonPropertyName("price_egp")]
    public decimal PriceEgp { get; set; }
    [JsonPropertyName("price_dollar")]
    public decimal PriceDollar { get; set; }
    public bool Available { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("monthly_points")]
    public int MonthlyPoints { get; set; }
    [JsonPropertyName("limit_daily")]
    public int LimitDaily { get; set; }
    [JsonPropertyName("limit_monthly")]
    public int LimitMonthly { get; set; }
    [JsonPropertyName("best_sell")]
    public bool BestSell { get; set; }
}

public class UpdateProfileRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

public class ChangePasswordRequest
{
    public string NewPassword { get; set; } = string.Empty;
}

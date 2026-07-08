using System;
using System.Collections.Generic;

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

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresInHours { get; set; }
    public UserInfo User { get; set; } = default!;
}

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public decimal Balance { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ApiToken { get; set; }
    public int ProfileId { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int Messages { get; set; }
    public bool PayAsYouGo { get; set; }
    public bool Expired { get; set; }
    public int UsedPoints { get; set; }
}

public class ProfileResponse
{
    public UserInfo User { get; set; } = default!;
    public ProfileInfo Profile { get; set; } = default!;
}

public class ProfileInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public int Days { get; set; }
    public string Type { get; set; } = string.Empty;
    public int DailyPoints { get; set; }
    public decimal PriceEgp { get; set; }
    public decimal PriceDollar { get; set; }
    public bool Available { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MonthlyPoints { get; set; }
    public int LimitDaily { get; set; }
    public int LimitMonthly { get; set; }
    public int BestSell { get; set; }
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

using System;
using WhatsPro.Authentication.Models;
using WhatsPro.Clients.Models;
using WhatsPro.Dashboard.Models;
using WhatsPro.Models;

namespace WhatsPro.ConsoleSample;

/// <summary>
/// Provides richly formatted, color-coded console output for WhatsPro API responses.
/// </summary>
internal static class ConsoleDisplayHelper
{
    // ── Layout constants ──────────────────────────────────────────────────────
    private const int LabelWidth  = 18;
    private const int TotalWidth  = 52;
    private const char BorderChar = '═';
    private const char SectionChar = '─';

    // ── Color palette ─────────────────────────────────────────────────────────
    private static readonly ConsoleColor BorderColor  = ConsoleColor.DarkCyan;
    private static readonly ConsoleColor HeaderColor  = ConsoleColor.Cyan;
    private static readonly ConsoleColor SectionColor = ConsoleColor.DarkYellow;
    private static readonly ConsoleColor LabelColor   = ConsoleColor.Green;
    private static readonly ConsoleColor NullColor    = ConsoleColor.DarkGray;
    private static readonly ConsoleColor ValueColor   = ConsoleColor.White;

    // =========================================================================
    // Public API
    // =========================================================================

    /// <summary>Prints all <see cref="UserInfo"/> fields in grouped, colored sections.</summary>
    public static void PrintProfile(UserInfo user)
    {
        PrintDoubleBorder();
        PrintCenteredHeader("👤  User Profile");
        PrintDoubleBorder();
        Console.WriteLine();

        // ── Account ──────────────────────────────────────────────────────────
        PrintSectionHeader("Account");
        PrintField("ID",            user.Id.ToString());
        PrintField("Name",          user.Name);
        PrintField("Display Name",  user.DisplayName);
        PrintField("Email",         user.Email);
        PrintField("Phone",         user.Phone);
        PrintField("Country",       user.Country);
        PrintField("Bio",           user.Bio);
        PrintField("Avatar",        user.Avatar);
        PrintField("Admin",         user.IsAdmin  ? "Yes" : "No");
        PrintField("Verified",      user.Verified == 1 ? "Yes" : "No");
        PrintField("Created At",    user.CreatedAt.ToString("yyyy-MM-dd"));
        PrintField("Expires At",    user.ExpiresAt.HasValue
                                        ? user.ExpiresAt.Value.ToString("yyyy-MM-dd")
                                        : null);
        PrintField("Expired",       user.Expired ? "Yes" : "No");
        Console.WriteLine();

        // ── Usage ─────────────────────────────────────────────────────────────
        PrintSectionHeader("Usage");
        PrintField("Balance",       $"{user.Balance:F2}");
        PrintField("Messages Sent", user.Messages.ToString());
        PrintField("Used Points",   user.UsedPoints.ToString());
        PrintField("Daily Usage",   $"{user.PointsUsageDaily} pts");
        PrintField("Monthly Usage", $"{user.PointsUsageMonthly} pts");
        PrintField("Pay-As-You-Go", user.PayAsYouGo.ToString());
        PrintField("Max Sessions",  user.MaxSessions.ToString());
        PrintField("Webhook URL",   user.WebhookUrl);
        PrintField("API Token",     user.ApiToken);
        PrintField("2FA",           user.TwoFactorEnabledAt.HasValue
                                        ? $"Enabled ({user.TwoFactorEnabledAt.Value:yyyy-MM-dd})"
                                        : "Disabled");
        PrintField("Paused At",     user.PausedAt.HasValue
                                        ? user.PausedAt.Value.ToString("yyyy-MM-dd")
                                        : null);
        Console.WriteLine();

        // ── Subscription Plan ─────────────────────────────────────────────────
        PrintSectionHeader("Subscription Plan");
        if (user.Profile is { } plan)
        {
            PrintField("Plan ID",       plan.Id.ToString());
            PrintField("Name",          plan.Name);
            PrintField("Details",       plan.Details);
            PrintField("Type",          plan.Type);
            PrintField("Days",          plan.Days.ToString());
            PrintField("Daily Limit",   $"{plan.DailyPoints} pts");
            PrintField("Monthly Limit", $"{plan.MonthlyPoints} pts");
            PrintField("Price (EGP)",   $"{plan.PriceEgp:F2}");
            PrintField("Price (USD)",   $"{plan.PriceDollar:F2}");
            PrintField("Available",     plan.Available ? "Yes" : "No");
            PrintField("Best Seller",   plan.BestSell  ? "Yes" : "No");
        }
        else
        {
            PrintValue("N/A");
        }

        Console.WriteLine();
        PrintDoubleBorder();
        Console.WriteLine();
    }

    /// <summary>Prints a formatted table of clients with pagination info.</summary>
    public static void PrintClients(PagedResponse<ClientInfo> paged)
    {
        Console.WriteLine();
        Write(SectionColor, $"── Clients ");
        Write(ConsoleColor.Gray, $"({paged.Total} total, page {paged.CurrentPage}/{paged.LastPage})");
        Console.WriteLine();

        // Table header
        Write(LabelColor, $"  {"ID",-6}  {"Name",-22}  {"Phone",-16}  {"Group",-16}  Notes");
        Console.WriteLine();
        Write(BorderColor, "  " + new string(SectionChar, 6) + "  " + new string(SectionChar, 22) + "  " + new string(SectionChar, 16) + "  " + new string(SectionChar, 16) + "  " + new string(SectionChar, 12));
        Console.WriteLine();

        foreach (var c in paged.Data)
        {
            Write(NullColor,  $"  [{c.Id,-4}]  ");
            Write(ValueColor, $"{Truncate(c.Name, 22),-22}  ");
            Write(ValueColor, $"{Truncate(c.Phone, 16),-16}  ");
            Write(ValueColor, $"{Truncate(c.Group?.Name ?? "—", 16),-16}  ");

            var notes = string.IsNullOrWhiteSpace(c.Notes) ? null : c.Notes;
            PrintValue(notes);
        }

        Console.WriteLine();
    }

    /// <summary>Prints dashboard cards and percentage statistics.</summary>
    public static void PrintDashboard(DashboardResponse data)
    {
        Console.WriteLine();

        // ── Cards ─────────────────────────────────────────────────────────────
        PrintSectionHeader("Dashboard Cards");
        foreach (var card in data.Cards)
        {
            PrintField(card.Name, card.Value);
        }
        Console.WriteLine();

        // ── Statistics ────────────────────────────────────────────────────────
        PrintSectionHeader("Statistics");
        foreach (var pct in data.Percentage)
        {
            var bar     = BuildProgressBar(pct.Data.Percentage, barWidth: 20);
            var trend   = pct.MoreIsBetter ? "↑" : "↓";
            var current = pct.Data.Current.ToString("F0");
            var limit   = pct.Data.Limit.ToString("F0");
            var percent = pct.Data.Percentage.ToString("F1");

            Write(LabelColor, $"  {pct.Name.PadRight(LabelWidth)}: ");
            Write(ValueColor, $"{current,6} / {limit,6}  ");
            Write(NullColor,  $"[");
            Write(ConsoleColor.DarkGreen, bar);
            Write(NullColor,  $"] ");
            Write(ValueColor, $"{percent,5}%  {trend} ");
            Write(ConsoleColor.DarkGray, pct.Details);
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    // =========================================================================
    // Private helpers
    // =========================================================================

    private static void PrintDoubleBorder()
    {
        Write(BorderColor, new string(BorderChar, TotalWidth));
        Console.WriteLine();
    }

    private static void PrintCenteredHeader(string text)
    {
        var padded = text.PadLeft((TotalWidth + text.Length) / 2).PadRight(TotalWidth);
        Write(HeaderColor, padded);
        Console.WriteLine();
    }

    private static void PrintSectionHeader(string title)
    {
        var line = $"── {title} " + new string(SectionChar, Math.Max(0, TotalWidth - title.Length - 4));
        Write(SectionColor, line);
        Console.WriteLine();
    }

    private static void PrintField(string label, string? value)
    {
        Write(LabelColor, $"  {label.PadRight(LabelWidth)}: ");
        PrintValue(value);
    }

    private static void PrintValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Write(NullColor, "N/A");
            Console.WriteLine();
        }
        else
        {
            Write(ValueColor, value!);
            Console.WriteLine();
        }
    }

    private static void Write(ConsoleColor color, string text)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.Write(text ?? string.Empty);
        Console.ForegroundColor = prev;
    }

    private static string Truncate(string? value, int max)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        var safe = value!;
        return safe.Length <= max ? safe : safe.Substring(0, max - 1) + "\u2026";
    }

    private static string BuildProgressBar(decimal percentage, int barWidth)
    {
        var filled = (int)Math.Round((double)percentage / 100 * barWidth);
        if (filled < 0) filled = 0;
        if (filled > barWidth) filled = barWidth;
        return new string('\u2588', filled) + new string('\u2591', barWidth - filled);
    }
}

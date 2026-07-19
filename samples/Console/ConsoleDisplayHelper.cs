using System;
using System.Text.Json;
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

    /// <summary>Prints all fields of a single ClientInfo, including phones and group details.</summary>
    public static void PrintClientDetail(WhatsPro.Clients.Models.ClientInfo c)
    {
        PrintDoubleBorder();
        PrintCenteredHeader("👤  Client Details");
        PrintDoubleBorder();
        Console.WriteLine();

        PrintSectionHeader("Client");
        PrintField("ID",          c.Id.ToString());
        PrintField("Name",        c.Name);
        PrintField("Phone",       c.Phone);
        PrintField("Notes",       c.Notes);
        PrintField("Phones Count",c.PhonesCount.ToString());
        PrintField("Created At",  c.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        PrintField("Updated At",  c.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        Console.WriteLine();

        if (c.Group != null)
        {
            PrintSectionHeader("Group Info");
            PrintField("Group ID",   c.Group.Id.ToString());
            PrintField("Group Name", c.Group.Name);
            PrintField("Group Notes",c.Group.Notes);
            Console.WriteLine();
        }

        if (c.Phones != null && c.Phones.Count > 0)
        {
            PrintSectionHeader("Extra Phones");
            Write(LabelColor, $"  {"ID",-8}  {"Phone",-16}  {"Default",-8}");
            Console.WriteLine();
            Write(BorderColor, "  " + new string(SectionChar, 8) + "  " + new string(SectionChar, 16) + "  " + new string(SectionChar, 8));
            Console.WriteLine();

            foreach (var p in c.Phones)
            {
                Write(NullColor,  $"  [{p.Id,-6}]  ");
                Write(ValueColor, $"{Truncate(p.Phone, 16),-16}  ");
                Write(ValueColor, $"{(p.Default ? "Yes" : "No"),-8}");
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        PrintDoubleBorder();
        Console.WriteLine();
    }

    /// <summary>Prints a formatted table of sessions with pagination info.</summary>
    public static void PrintSessions(PagedResponse<WhatsPro.Sessions.Models.SessionInfo> paged)
    {
        Console.WriteLine();
        Write(SectionColor, $"── Sessions ");
        Write(ConsoleColor.Gray, $"({paged.Total} total, page {paged.CurrentPage}/{paged.LastPage})");
        Console.WriteLine();

        // Table header
        Write(LabelColor, $"  {"ID",-6}  {"Name",-16}  {"Phone",-16}  Status");
        Console.WriteLine();
        Write(BorderColor, "  " + new string(SectionChar, 6) + "  " + new string(SectionChar, 16) + "  " + new string(SectionChar, 16) + "  " + new string(SectionChar, 12));
        Console.WriteLine();

        foreach (var s in paged.Data)
        {
            Write(NullColor,  $"  [{s.Id,-4}]  ");
            Write(ValueColor, $"{Truncate(s.Name, 16),-16}  ");
            Write(ValueColor, $"{Truncate(string.IsNullOrEmpty(s.Phone) ? "—" : s.Phone, 16),-16}  ");
            Write(ValueColor, $"{s.Status}");
            Console.WriteLine();
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
            PrintField(card.Name, CardValueToString(card.Value));
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

    /// <summary>Prints send chart data.</summary>
    public static void PrintSendChart(System.Collections.Generic.List<int> data)
    {
        Console.WriteLine();
        PrintSectionHeader("Send Chart (Messages by Day)");
        for (int i = 0; i < data.Count; i++)
        {
            var day = i + 1;
            Write(LabelColor, $"  Day {day,2}: ");
            Write(ValueColor, $"{data[i],5} msgs");
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    /// <summary>Prints top numbers statistics.</summary>
    public static void PrintTopNumbers(System.Collections.Generic.List<TopNumberItem> data)
    {
        Console.WriteLine();
        PrintSectionHeader("Top Numbers");
        
        Write(LabelColor, $"  {"Phone / Name",-20}  {"Messages",-10}  {"Has Doc",-8}");
        Console.WriteLine();
        Write(BorderColor, "  " + new string(SectionChar, 20) + "  " + new string(SectionChar, 10) + "  " + new string(SectionChar, 8));
        Console.WriteLine();

        foreach (var num in data)
        {
            Write(ValueColor, $"  {Truncate(num.Name, 20),-20}  ");
            Write(ValueColor, $"{num.Value,-10}  ");
            Write(ValueColor, $"{(num.HasDocument ? "Yes" : "No"),-8}");
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    /// <summary>Prints a formatted table of messages with pagination info.</summary>
    public static void PrintMessages(WhatsPro.Models.PagedResponse<WhatsPro.Messages.Models.MessageInfo> paged)
    {
        Console.WriteLine();
        Write(SectionColor, $"── Messages ");
        Write(ConsoleColor.Gray, $"({paged.Total} total, page {paged.CurrentPage}/{paged.LastPage})");
        Console.WriteLine();

        Write(LabelColor, $"  {"ID",-10}  {"To",-16}  {"Status",-10}  {"Channel",-10}  Message");
        Console.WriteLine();
        Write(BorderColor, "  " + new string(SectionChar, 10) + "  " + new string(SectionChar, 16) + "  " + new string(SectionChar, 10) + "  " + new string(SectionChar, 10) + "  " + new string(SectionChar, 28));
        Console.WriteLine();

        foreach (var m in paged.Data)
        {
            Write(NullColor,  $"  [{m.Id,-8}]  ");
            Write(ValueColor, $"{Truncate(m.To, 16),-16}  ");

            var statusColor = m.Status switch
            {
                "read"      => ConsoleColor.Green,
                "delivered" => ConsoleColor.Cyan,
                "sent"      => ConsoleColor.Yellow,
                _           => ConsoleColor.DarkGray
            };
            Write(statusColor, $"{m.Status,-10}  ");
            Write(ConsoleColor.DarkMagenta, $"{m.Channel,-10}  ");
            Write(ValueColor, Truncate(m.Message, 28));
            Console.WriteLine();
        }

        Console.WriteLine();
    }

    /// <summary>Prints all fields of a single MessageInfo.</summary>
    public static void PrintMessageDetail(WhatsPro.Messages.Models.MessageInfo m)
    {
        PrintDoubleBorder();
        PrintCenteredHeader("✉  Message Details");
        PrintDoubleBorder();
        Console.WriteLine();

        PrintSectionHeader("Message");
        PrintField("ID",          m.Id.ToString());
        PrintField("Message ID",  m.MessageId);
        PrintField("From",        m.From);
        PrintField("To",          m.To);
        PrintField("Channel",     m.Channel);
        PrintField("Status",      m.Status);
        PrintField("Created At",  m.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        PrintField("Updated At",  m.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        Console.WriteLine();

        PrintSectionHeader("Content");
        PrintField("Text",        m.Message);
        PrintField("Image URL",   m.ImgUrl);
        PrintField("Has Document",m.HasDocument ? "Yes" : "No");
        PrintField("Doc Name",    m.DocumentName);
        PrintField("Doc MIME",    m.DocumentMime);

        Console.WriteLine();
        PrintDoubleBorder();
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

    private static string? CardValueToString(JsonElement? element)
    {
        if (element is null) return null;
        return element.Value.ValueKind switch
        {
            JsonValueKind.Null      => null,
            JsonValueKind.Undefined => null,
            JsonValueKind.String    => element.Value.GetString(),
            JsonValueKind.Number    => element.Value.GetDecimal().ToString("G"),
            JsonValueKind.True      => "true",
            JsonValueKind.False     => "false",
            _                       => element.Value.ToString()
        };
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

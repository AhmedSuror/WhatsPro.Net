using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhatsPro.Http;

/// <summary>
/// Provides default JSON serialization options for the SDK.
/// </summary>
internal static class JsonOptions
{
    /// <summary>
    /// Gets the default <see cref="JsonSerializerOptions"/>.
    /// </summary>
    public static JsonSerializerOptions Default { get; } = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(),
            new ApiDateTimeConverter(),
            new IntOrBoolConverter()
        }
    };
}

/// <summary>
/// Parses <see cref="DateTime"/> strings in the <c>"yyyy-MM-dd HH:mm:ss"</c> format
/// returned by the WhatsPro API. Falls back to standard ISO 8601 for any other format.
/// </summary>
internal sealed class ApiDateTimeConverter : JsonConverter<DateTime>
{
    private const string ApiFormat = "yyyy-MM-dd HH:mm:ss";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var s = reader.GetString();
        if (string.IsNullOrEmpty(s))
            return default;

        if (DateTime.TryParseExact(s, ApiFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var result))
            return result;

        // Fallback: standard ISO 8601 ("2024-08-12T17:44:50" etc.)
        return DateTime.Parse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString(ApiFormat, CultureInfo.InvariantCulture));
}

/// <summary>
/// Handles <see cref="bool"/> values that the WhatsPro API may return as JSON integers
/// (<c>0</c> / <c>1</c>) instead of proper JSON <c>true</c> / <c>false</c> tokens.
/// </summary>
internal sealed class IntOrBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:  return true;
            case JsonTokenType.False: return false;
            case JsonTokenType.Number: return reader.GetInt32() != 0;
            default:
                throw new JsonException(
                    $"Cannot convert JSON token '{reader.TokenType}' to Boolean.");
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteBooleanValue(value);
}

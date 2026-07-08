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
        Converters = { new JsonStringEnumConverter() }
    };
}

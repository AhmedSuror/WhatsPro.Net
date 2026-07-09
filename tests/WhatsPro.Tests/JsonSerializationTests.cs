using System;
using System.Text.Json;
using WhatsPro.Http;
using WhatsPro.Models;

namespace WhatsPro.Tests;

public class JsonSerializationTests
{
    [Fact]
    public void PaginationRequest_SerializesToCamelCase()
    {
        // Arrange
        var request = new PaginationRequest
        {
            Count = 15,
            Page = 1,
            Search = "test"
        };

        // Act
        string json = JsonSerializer.Serialize(request, JsonOptions.Default);

        // Assert
        Assert.Contains("\"count\":15", json);
        Assert.Contains("\"page\":1", json);
        Assert.Contains("\"search\":\"test\"", json);
        Assert.DoesNotContain("Count", json);
    }
}

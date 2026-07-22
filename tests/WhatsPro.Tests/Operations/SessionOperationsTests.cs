using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WhatsPro.Sessions.Models;
using Xunit;

namespace WhatsPro.Tests.Operations;

public class SessionOperationsTests
{
    [Fact]
    public async Task GetAsync_ReturnsSessionInfo()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Get, "https://api.mock.whatspro.net/sessions/2", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 2, \"status\": \"connected\" } }");

        // Act
        var response = await client.Sessions.GetAsync(2);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("connected", response.Data.Status);
    }

    [Fact]
    public async Task ConnectAsync_ReturnsSessionInfo()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Get, "https://api.mock.whatspro.net/sessions/connect/2", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 2, \"status\": \"qr_ready\", \"qr_code\": \"base64data\" } }");

        // Act
        var response = await client.Sessions.ConnectAsync(2);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("qr_ready", response.Data.Status);
    }
}

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WhatsPro.Messages.Models;
using Xunit;

namespace WhatsPro.Tests.Operations;

public class MessageOperationsTests
{
    [Fact]
    public async Task GetAsync_ReturnsMessageInfo()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Get, "https://api.mock.whatspro.net/user/messages/100", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 100, \"message\": \"Hello!\" } }");

        // Act
        var response = await client.Messages.GetAsync(100);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Hello!", response.Data.Message);
    }

    [Fact]
    public async Task SendAsync_ReturnsSuccessString()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Post, "https://api.mock.whatspro.net/user/messages/send", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": \"Message queued successfully.\" }");

        // Act
        var request = new SendMessageRequest { ClientIds = new System.Collections.Generic.List<int> { 10 }, Message = "Hello!" };
        var response = await client.Messages.SendAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Equal("Message queued successfully.", response.Data);
    }
}

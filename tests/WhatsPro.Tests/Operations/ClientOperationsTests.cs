using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WhatsPro.Clients.Models;
using Xunit;

namespace WhatsPro.Tests.Operations;

public class ClientOperationsTests
{
    [Fact]
    public async Task GetAsync_ReturnsClientInfo()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Get, "https://api.mock.whatspro.net/clients/10", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 10, \"name\": \"Test Client\", \"group_id\": 5 } }");

        // Act
        var response = await client.Clients.GetAsync(10);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Test Client", response.Data.Name);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNewClientId()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Post, "https://api.mock.whatspro.net/clients", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 99 } }");

        // Act
        var request = new CreateClientRequest { Name = "New Client", GroupId = 1 };
        var response = await client.Clients.CreateAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(99, response.Data.Id);
    }
}

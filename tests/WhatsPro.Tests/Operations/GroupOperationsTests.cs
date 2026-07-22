using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WhatsPro.Groups.Models;
using Xunit;

namespace WhatsPro.Tests.Operations;

public class GroupOperationsTests
{
    [Fact]
    public async Task GetAsync_ReturnsGroupInfo()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Get, "https://api.mock.whatspro.net/groups/5", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 5, \"name\": \"Test Group\" } }");

        // Act
        var response = await client.Groups.GetAsync(5);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("Test Group", response.Data.Name);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNewGroupResponse()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Post, "https://api.mock.whatspro.net/groups", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 8 } }");

        // Act
        var request = new CreateGroupRequest { Name = "New Group" };
        var response = await client.Groups.CreateAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal(8, response.Data.Id);
    }
}

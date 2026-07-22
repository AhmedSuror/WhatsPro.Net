using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WhatsPro.Tests.Operations;

public class DashboardOperationsTests
{
    [Fact]
    public async Task GetDashboardAsync_ReturnsDashboardResponse()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Get, "https://api.mock.whatspro.net/user/dashboard", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"total_messages\": 500, \"failed_messages\": 10 } }");

        // Act
        var response = await client.Dashboard.GetDashboardAsync();

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
    }
}

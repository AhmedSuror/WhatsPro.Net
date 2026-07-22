using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WhatsPro.Authentication.Models;
using Xunit;

namespace WhatsPro.Tests.Operations;

public class AuthOperationsTests
{
    [Fact]
    public async Task LoginAsync_ReturnsToken_WhenSuccessful()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        
        // Act
        var request = new LoginRequest { Email = "test@example.com", Password = "password" };
        var response = await client.Auth.LoginAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.NotNull(response.Data);
        Assert.Equal("test-token", response.Data.AccessToken);
        Assert.NotNull(response.Data.User);
        Assert.Equal("test-api-token", response.Data.User.ApiToken);
        
        // Ensure the handler recorded the request
        Assert.Contains(handler.Requests, r => r.Method == HttpMethod.Post && r.RequestUri?.ToString().EndsWith("/user/login") == true);
    }

    [Fact]
    public async Task GetProfileAsync_ReturnsUserInfo()
    {
        // Arrange
        var (client, handler) = TestHelper.CreateMockClient();
        handler.Setup(HttpMethod.Get, "https://api.mock.whatspro.net/user/profile", HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"id\": 123, \"email\": \"test@example.com\", \"name\": \"John Doe\" } }");

        // Act
        var response = await client.Auth.GetProfileAsync();

        // Assert
        Assert.True(response.Success);
        Assert.Equal("test@example.com", response.Data.Email);
        Assert.Equal("John Doe", response.Data.Name);
    }
}

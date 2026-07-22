using System.Net.Http;
using WhatsPro.Tests.Http;

namespace WhatsPro.Tests;

internal static class TestHelper
{
    public static (WhatsProClient Client, MockHttpMessageHandler Handler) CreateMockClient()
    {
        var options = new WhatsProOptions
        {
            BaseUrl = "https://api.mock.whatspro.net",
            Email = "test@example.com",
            Password = "password",
            EncryptionKey = "0123456789abcdef0123456789abcdef" // 32 chars for AES
        };

        var handler = new MockHttpMessageHandler();
        
        // Mock authentication by default if not overridden
        handler.Setup(HttpMethod.Post, "https://api.mock.whatspro.net/user/login", System.Net.HttpStatusCode.OK, 
            "{ \"success\": true, \"data\": { \"access_token\": \"test-token\", \"user\": { \"api_token\": \"test-api-token\" } } }");

        var client = new WhatsProClient(options, handler);
        return (client, handler);
    }
}

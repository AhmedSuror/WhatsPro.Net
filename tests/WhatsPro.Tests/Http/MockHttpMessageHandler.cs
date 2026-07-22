using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WhatsPro.Tests.Http;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, HttpResponseMessage> _mockResponses = new();
    public List<HttpRequestMessage> Requests { get; } = new();

    public void Setup(HttpMethod method, string url, HttpStatusCode statusCode, string jsonResponse)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };
        _mockResponses[$"{method}:{url}"] = response;
    }
    
    public void Setup(HttpMethod method, string url, HttpResponseMessage response)
    {
        _mockResponses[$"{method}:{url}"] = response;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request);
        string key = $"{request.Method}:{request.RequestUri?.ToString()}";
        
        if (_mockResponses.TryGetValue(key, out var response))
        {
            var clonedResponse = new HttpResponseMessage(response.StatusCode);
            if (response.Content != null)
            {
                var contentBytes = response.Content.ReadAsByteArrayAsync().GetAwaiter().GetResult();
                clonedResponse.Content = new ByteArrayContent(contentBytes);
                foreach (var header in response.Content.Headers)
                {
                    clonedResponse.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
            return Task.FromResult(clonedResponse);
        }

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) 
        { 
            Content = new StringContent($"No mock found for {key}") 
        });
    }
}

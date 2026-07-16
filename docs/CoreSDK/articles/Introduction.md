# Introduction to WhatsPro.Net

Welcome to the **WhatsPro.Net SDK** usage guide! This documentation will teach you how to integrate your .NET applications with the WhatsPro API efficiently. 

## What is WhatsPro.Net?

WhatsPro.Net is an unofficial, strongly-typed .NET SDK for interacting with the WhatsPro REST API. It wraps complex HTTP calls, handles JWT authentication, automatically renews expired tokens, and provides clean C# models for all endpoints.

## Why use this SDK?

When building apps that send WhatsApp messages, managing authentication tokens, constructing JSON payloads, and handling multipart form data (for Excel imports or image uploads) can be tedious. This SDK takes care of the heavy lifting:
* **Strongly-typed models:** Never guess the JSON schema. Everything from `DashboardResponse` to `ClientInfo` is modeled.
* **Smart Authentication:** The `WhatsProHttpClient` handles JWT lifecycle automatically.
* **Easy DI Integration:** Designed to work perfectly with .NET Core Dependency Injection.

## Setup and Installation

Currently, you can integrate this SDK into your solution by referencing the `WhatsPro` project, or by installing the NuGet package (if you have published it).

### Basic Configuration

The entry point for the SDK is the `WhatsProClient` class. It requires a `WhatsProOptions` object containing your account credentials and the base URL.

```csharp
using WhatsPro;

var options = new WhatsProOptions
{
    BaseUrl = "https://whats-pro.net/backend/public/index.php/api",
    Email = "your_email@example.com",
    Password = "your_password"
};

// Important: Wrap the client in a using statement to dispose of the underlying HttpClient properly.
using var client = new WhatsProClient(options);
```

### Dependency Injection (Advanced Setup)

For modern ASP.NET Core apps or worker services, we recommend registering `WhatsProClient` as a Singleton or Scoped service depending on your architecture. Because `WhatsProClient` creates its own `HttpClient`, you might want to wrap it or inject it simply.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton(new WhatsProOptions
    {
        BaseUrl = "https://whats-pro.net/backend/public/index.php/api",
        Email = "your_email@example.com",
        Password = "your_password"
    });

    services.AddTransient<WhatsProClient>();
}
```

Now you can inject `WhatsProClient` directly into your controllers or services!

## Navigating the Guide

Check out the sidebar to explore specific modules. If you're building an integration that sends notifications, start with **Messages**. If you're building a CRM sync tool, look at **Clients** and **Groups**.

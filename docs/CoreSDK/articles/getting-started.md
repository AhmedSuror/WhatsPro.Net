# Getting Started with WhatsPro.Net

## Installation
Install via NuGet:
```bash
dotnet add package WhatsPro.Net
```

## Basic Usage
```csharp
using WhatsPro;

var options = new WhatsProOptions 
{ 
    BaseUrl = "https://whats-pro.net/backend/public/index.php/api",
    Email = "your-email",
    Password = "your-password"
};

using var client = new WhatsProClient(options);
var profile = await client.Auth.GetProfileAsync();
Console.WriteLine($"Logged in as {profile.Data.User.Name}");
```

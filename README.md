# WhatsPro.Net

A non-official .NET SDK library for the Whats-Pro.net REST API.

## Features
- Clean and intuitive API
- **Strongly Typed Models**: Clean C# classes for all request and response structures.
- **Robust Authentication**: Thread-safe JWT caching and auto-renewal under the hood.
- **Dashboard API**: Fetch live statistics, charts, and top metrics easily.
- **Clients & Groups**: Fully typed endpoints to manage clients and groups, including Excel imports.
- **Sessions & Messages**: Connect multiple WhatsApp instances and send messages seamlessly.
- **Multi-targeting**: Supports both `.NET Standard 2.0` and `.NET 4.8`.

## Quick Start

```csharp
using WhatsPro;

var options = new WhatsProOptions
{
    BaseUrl = "https://whats-pro.net/backend/public/index.php/api",
    Email = "your_email@example.com",
    Password = "your_password"
};

using var client = new WhatsProClient(options);

// Automatically handles authentication & payload encryption
var profile = await client.Auth.GetProfileAsync();
Console.WriteLine($"Logged in as: {profile.Data.User.Name}");
```

## Contributing
Please see `CONTRIBUTING.md` for details on contributing to the project.

## License
This project is licensed under the Apache License 2.0 - see the `LICENSE` file for details.

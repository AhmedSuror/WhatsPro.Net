# WhatsPro.Net

A non-official .NET SDK library for the Whats-Pro.net REST API.

## Features
- Clean and intuitive API
- Strongly typed models
- Built-in AES encryption
- Fully asynchronous
- Support for `net48` and `netstandard2.0`

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
var dashboard = await client.Dashboard.GetDashboardAsync();
Console.WriteLine($"Cards: {dashboard.Data.Cards.Count}");
```

## Contributing
Please see `CONTRIBUTING.md` for details on contributing to the project.

## License
This project is licensed under the Apache License 2.0 - see the `LICENSE` file for details.

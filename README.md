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

## Full SDK Usage Guide

We provide a comprehensive, tutorial-style usage guide for all modules in the SDK:
- [Introduction & Setup](docs/Introduction.md)
- [Authentication](docs/Authentication.md)
- [Messages](docs/Messages.md)
- [Clients](docs/Clients.md)
- [Groups](docs/Groups.md)
- [Sessions](docs/Sessions.md)
- [Dashboard](docs/Dashboard.md)

## Generating and Publishing Documentation

This repository uses [DocFX](https://dotnet.github.io/docfx/) to generate a beautiful, interactive documentation site that includes both our **Usage Guides** and **API Reference**.

### Local Generation
To build the documentation locally:
1. Install DocFX: `dotnet tool update -g docfx`
2. Run DocFX in the repository root: `docfx docfx.json --serve`
3. Navigate to `http://localhost:8080` to view the site.

### Publishing to GitHub Pages with a Custom Subdomain

We recommend hosting your documentation on **GitHub Pages**, mapped to your custom subdomain (e.g., `docs.yourwebsite.com`).

**Step 1: Set up a GitHub Action**
You can create a GitHub Action (`.github/workflows/docs.yml`) that runs `docfx` and pushes the `_site` output to the `gh-pages` branch on every push to `main`.

**Step 2: Configure your Custom Domain**
1. Go to your repository **Settings** > **Pages**.
2. Under **Build and deployment**, select "Deploy from a branch" and choose the `gh-pages` branch.
3. Under **Custom domain**, enter your desired subdomain (e.g., `docs.whatspro.net`) and click **Save**. This will automatically create a `CNAME` commit in your `gh-pages` branch.
4. Go to your domain registrar (e.g., GoDaddy, Cloudflare) and create a **CNAME record** pointing your subdomain (`docs`) to `<your-github-username>.github.io`.

Once DNS propagation completes, your full SDK documentation will be live on your custom domain!

## Contributing
Please see `CONTRIBUTING.md` for details on contributing to the project.

## License
This project is licensed under the Apache License 2.0 - see the `LICENSE` file for details.

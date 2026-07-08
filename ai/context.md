# init.md

# WhatsPro.Net SDK

## Overview

Build a professional, production-ready .NET SDK for interacting with the
Whats-Pro REST API.

The SDK must be completely standalone and application-agnostic.

It should be designed as if it will be maintained for years and consumed
by third-party developers through GitHub and potentially NuGet.

------------------------------------------------------------------------

# Project Goals

-   Clean and intuitive API
-   Strongly typed models
-   Minimal external dependencies
-   High maintainability
-   Excellent developer experience
-   Production-ready code
-   Well documented
-   Fully asynchronous
-   Easy to extend

------------------------------------------------------------------------

# Project Philosophy

This project is **NOT** a simple HTTP wrapper.

It is a complete SDK.

Consumers should never have to know:

-   API endpoints
-   HTTP verbs
-   Authentication headers
-   JSON serialization
-   Request formatting
-   Response parsing

Instead, they should interact with a clean object-oriented API.

Good:

``` csharp
var client = new WhatsProClient(options);

await client.Messages.SendTextAsync(...);
await client.Sessions.CreateAsync(...);
```

Bad:

``` csharp
client.Post("/api/messages/send", body);
client.CallEndpoint(...);
client.Send(...);
```

------------------------------------------------------------------------

# Current Scope

The first release targets only Whats-Pro.

-   No provider abstraction
-   No interfaces
-   No Dependency Injection
-   No multi-provider architecture

Keep the SDK focused and stable.

------------------------------------------------------------------------

# Target Frameworks

Support:

-   net48
-   netstandard2.0

------------------------------------------------------------------------

# Repository Structure

``` text
WhatsPro.Net/
│
├── .github/
│   ├── workflows/
│   ├── ISSUE_TEMPLATE/
│   ├── PULL_REQUEST_TEMPLATE.md
│   └── dependabot.yml
│
├── docs/
│
├── eng/
│
├── samples/
│   └── Console/
│
├── src/
│   └── WhatsPro/
│
├── tests/
│   └── WhatsPro.Tests/
│
├── .editorconfig
├── .gitignore
├── CHANGELOG.md
├── CONTRIBUTING.md
├── Directory.Build.props
├── Directory.Build.targets
├── global.json
├── LICENSE
├── README.md
├── init.md
└── WhatsPro.Net.sln
```

The solution file must remain at the repository root.

------------------------------------------------------------------------

# Namespace

``` csharp
namespace WhatsPro;
```

Sub-namespaces:

``` text
WhatsPro.Models
WhatsPro.Messages
WhatsPro.Sessions
WhatsPro.Users
WhatsPro.Groups
WhatsPro.Webhooks
WhatsPro.Exceptions
WhatsPro.Internal
```

------------------------------------------------------------------------

# Folder Structure

``` text
Authentication/
Configuration/
Messages/
Sessions/
Users/
Groups/
Webhooks/
Models/
Exceptions/
Serialization/
Internal/
Utilities/
```

------------------------------------------------------------------------

# Public API Design

``` csharp
await client.Messages.SendTextAsync(...);
await client.Messages.SendImageAsync(...);

await client.Sessions.CreateAsync(...);
await client.Sessions.GetAsync(...);

await client.Webhooks.SetAsync(...);
```

Avoid exposing dozens of unrelated methods directly on `WhatsProClient`.

------------------------------------------------------------------------

# Configuration

``` csharp
var options = new WhatsProOptions
{
    BaseUrl = "...",
    Token = "..."
};

var client = new WhatsProClient(options);
```

No static configuration.

No global state.

------------------------------------------------------------------------

# HTTP Layer

Use only:

-   HttpClient

Do NOT use:

-   RestSharp
-   Flurl
-   Refit

Create an internal transport layer responsible for:

-   Request creation
-   Authentication
-   Sending requests
-   Serialization
-   Error mapping
-   Response parsing

------------------------------------------------------------------------

# Serialization

Use:

-   System.Text.Json

------------------------------------------------------------------------

# HttpClient

Support constructors similar to:

``` csharp
new WhatsProClient(options);
new WhatsProClient(httpClient);
new WhatsProClient(httpClient, options);
```

Never create a new HttpClient per request.

------------------------------------------------------------------------

# Exceptions

Expose strongly typed exceptions only.

Examples:

-   WhatsProException
-   AuthenticationException
-   ApiException
-   ValidationException
-   NetworkException

Never expose raw HttpResponseMessage.

------------------------------------------------------------------------

# Models

Every request and response should have a strongly typed model.

Avoid:

-   dynamic
-   JObject
-   JsonDocument

------------------------------------------------------------------------

# Coding Standards

-   Async-first
-   Nullable Reference Types enabled
-   XML documentation on all public members
-   SOLID principles
-   Small focused classes
-   Minimal public surface
-   Prefer composition over inheritance

------------------------------------------------------------------------

# Internal Design

Each feature owns its implementation.

Examples:

-   Messages
-   Sessions
-   Users
-   Groups
-   Webhooks

Features should remain isolated.

------------------------------------------------------------------------

# Public API Stability

Treat every public type as a long-term contract.

Avoid unnecessary breaking changes.

Follow Semantic Versioning.

------------------------------------------------------------------------

# Logging

The SDK should not depend on any logging framework.

Provide extension points in the future if needed.

------------------------------------------------------------------------

# Dependency Injection

Out of scope.

If required later, create a separate package:

``` text
WhatsPro.DependencyInjection
```

------------------------------------------------------------------------

# Testing

Unit tests should cover:

-   Request generation
-   Response parsing
-   Serialization
-   Error handling
-   Exception mapping

------------------------------------------------------------------------

# Samples

Only one sample project is required for v1:

``` text
samples/
└── Console/
```

Every new feature must be demonstrated in the Console sample.

------------------------------------------------------------------------

# Documentation

Documentation is part of the product.

The repository must always contain a `README.md`.

Whenever a public feature changes:

-   Update README.md
-   Update documentation under `docs/`
-   Update Console sample
-   Ensure all examples compile
-   Keep documentation synchronized with implementation

Documentation must never become outdated.

README should contain:

-   Overview
-   Features
-   Installation
-   Supported Frameworks
-   Quick Start
-   Configuration
-   Examples
-   Version Compatibility
-   License
-   Contributing

------------------------------------------------------------------------

# License

Use the Apache License 2.0.

Include the official Apache 2.0 LICENSE file in the repository.

------------------------------------------------------------------------

# Continuous Integration

Prepare the repository for GitHub Actions.

Future CI should include:

-   Build
-   Unit Tests
-   Formatting
-   Package Validation

The project should build without warnings.

------------------------------------------------------------------------

# Code Quality

Prefer:

-   readability
-   maintainability
-   consistency
-   explicit code

Avoid clever code.

------------------------------------------------------------------------

# Future Packages

Out of scope for v1:

``` text
WhatsPro.DependencyInjection
WhatsPro.Extensions
WhatsPro.AspNetCore
WhatsPro.Mock
```

------------------------------------------------------------------------

# Important Rule

Design every public API as if this SDK will be published to NuGet and
used by thousands of developers.

Favor long-term maintainability over short-term convenience.

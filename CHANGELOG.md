# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [0.1.0-alpha.1] - 2026-07-09

### Added

- **Core SDK** — Initial `WhatsPro.Net` SDK scaffold with core HTTP infrastructure and `WhatsProClient` entry point.
- **Authentication** — Login and user-profile support with JWT token generation and transparent caching.
- **Security** — Auto-AES payload encryption integrated at the HTTP layer.
- **Multipart HTTP** — Multipart/form-data support for file-upload endpoints.
- **Dashboard** — Dashboard analytics operations module.
- **Clients** — Client management operations including Excel-based bulk import.
- **Groups** — Groups management operations module.
- **Sessions** — Session management operations module.
- **Messages** — Messaging operations module (send, list, etc.).
- **SDK surface** — `WhatsProClient` now exposes Sessions, Messages, Groups, Clients, Dashboard, and Authentication modules.
- **Error handling** — Enhanced structured error handling, custom exceptions, and input validations.
- **Tests** — Multi-targeted test suite covering `net48` and `netstandard2.0`.
- **Console Sample** — Interactive console sample application with user-secrets support and rich formatted output helpers.
- **Documentation** — GitHub Actions CI, contributing guide, issue templates, and detailed README.

### Fixed

- **HTTP** — Resolved base-address path concatenation issue that caused incorrect API endpoint URLs.

### Refactored

- **Auth models** — Flattened login and profile models/responses for a cleaner public API surface.

### Dependencies

- `System.Text.Json` bumped to 10.0.9
- `Microsoft.NET.Test.Sdk` bumped to 18.7.0
- `xunit` bumped to 2.9.3
- `xunit.runner.visualstudio` bumped to 3.1.5
- `coverlet.collector` bumped to 10.0.1
- GitHub Actions: `actions/checkout` to v7, `actions/setup-dotnet` to v5

---

## [Unreleased]

> Changes targeting the next release will be tracked here.

[0.1.0-alpha.1]: https://github.com/WhatsPro/WhatsPro.Net/releases/tag/v0.1.0-alpha.1
[Unreleased]: https://github.com/WhatsPro/WhatsPro.Net/compare/v0.1.0-alpha.1...HEAD

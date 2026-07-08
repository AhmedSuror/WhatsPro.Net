# WhatsPro.Net Copilot Instructions

## Architecture

Single-project SDK library (`src/WhatsPro/WhatsPro.csproj`) targeting `net48` and `netstandard2.0`, C# 12, nullable enabled, root namespace `WhatsPro`.

**Component map:**
| File | Visibility | Role |
|---|---|---|
| `WhatsProClient.cs` | `public` | Entry point — accepts `WhatsProOptions` or `Action<WhatsProOptions>` |
| `WhatsProOptions.cs` | `public` | Config POCO: `BaseUrl`, `Email`, `Password`, `EncryptionKey`, `Timeout` |
| `Http/WhatsProHttpClient.cs` | `internal` | `HttpClient` wrapper with generic verb methods |
| `Http/JsonOptions.cs` | `internal static` | Shared `JsonSerializerOptions` (camelCase, null-ignoring, enum-as-string) |
| `Security/PayloadEncryptor.cs` | `public static` | AES-CBC encrypt/decrypt, CryptoJS-compatible |

## Critical Patterns

### Encrypt-every-request
Every POST/PUT body follows this pipeline:
1. JSON serialize the request object using `JsonOptions.Default`
2. `PayloadEncryptor.Encrypt(json, _options.EncryptionKey)` → Base64 ciphertext
3. Wrap as `{ "payload": "<base64>" }` before sending

Responses reverse it: check for a `payload` JSON property → decrypt; fall back to direct decrypt on `JsonException`.

### Encryption scheme (CryptoJS-compatible AES-CBC)
`PayloadEncryptor` uses OpenSSL `EVP_BytesToKey` (MD5-iterated KDF) producing 32-byte key + 16-byte IV, and prefixes output with `Salted__` + 8-byte salt. The default key is `"abcd123456789ABCD"` (set in `WhatsProOptions`). Do not change the KDF or prefix — it must stay wire-compatible with CryptoJS.

### Multi-targeting guards
Use `#if NETSTANDARD2_0 || NET48` when the newer API is unavailable. Example:
```csharp
#if NETSTANDARD2_0 || NET48
using (var rng = RandomNumberGenerator.Create()) { rng.GetBytes(salt); }
#else
RandomNumberGenerator.Fill(salt);
#endif
```

### Async conventions (library)
- Always `ConfigureAwait(false)` on every `await` inside `WhatsProHttpClient`.
- Accept and forward `CancellationToken` in all HTTP methods.
- All async methods end with `Async`.

### Access levels
Keep new implementation types `internal`. Only expose via `WhatsProClient` (the public facade). Current `public` surface: `WhatsProClient`, `WhatsProOptions`, `PayloadEncryptor`.

## Adding New API Endpoints

1. Add `async Task<TResponse> VerbAsync<TRequest, TResponse>(string uri, ..., CancellationToken ct = default)` to `WhatsProHttpClient` following the existing pattern.
2. Expose it as a `public` method on `WhatsProClient`, uncommenting/wiring `_httpClient` as needed.
3. Use `JsonOptions.Default` for all serialization/deserialization.

## Build

```powershell
dotnet build src/WhatsPro/WhatsPro.csproj
```

Only NuGet dependency: `System.Text.Json 8.0.5`. `System.Net.Http` is a framework reference on `net48` only.

## Conventions

- XML `<summary>` doc comments on all `public` members.
- 4-space indent, UTF-8, final newline (`.editorconfig`).
- Null guards: `ArgumentNullException` for null constructor args; `ArgumentException` for blank option strings.
- `string.IsNullOrWhiteSpace` for option validation; `string.IsNullOrEmpty` inside crypto helpers.
- No interfaces unless needed for external dependency or test seam.

## README Maintenance

`README.md` is the public face of the library. **Update it in the same commit as the code change** — never leave it describing an outdated API.

| Code change | README section(s) to update |
|---|---|
| New API namespace added to `WhatsProClient` (e.g. `client.Messages`) | **Features** bullet + **Quick Start** example if it is the primary usage entry point |
| New or renamed `WhatsProOptions` property | **Quick Start** — the options block must stay buildable and representative |
| New target framework or dropped framework support | **Features** (`Support for …` bullet) |
| Changed public method signature or return type | **Quick Start** if the example calls that method |
| New top-level feature (e.g. retry policy, logging) | New bullet in **Features** |
| Renamed or removed public type | **Quick Start** code block — it must always compile against the current public surface |

### Quick Start rules
- The Quick Start example must always reflect a **real, working call** against the current public API.
- Show the minimal happy-path: construct `WhatsProOptions`, create `WhatsProClient`, call one meaningful method, print a result.
- Current shape: `client.<Namespace>.<MethodAsync>()` — keep that pattern as new namespaces are added.
- Use the canonical base URL `https://whats-pro.net/backend/public/index.php/api` as the placeholder.

## Git Commit Messages

When asked to generate a commit message, review the staged git diff and produce a professional message following the **Conventional Commits** specification.

Rules:
1. The first line must be a short summary (under 50 chars) in the format: `<type>(<optional scope>): <description in imperative mood>`
   (Types: `feat`, `fix`, `docs`, `style`, `refactor`, `perf`, `test`, `build`, `ci`, `chore`, `revert`).
2. Leave one blank line after the summary.
3. Provide a detailed, bulleted description of the changes. Focus on the **why** and **what**, not the literal code changes.
4. If there are breaking changes, include a `BREAKING CHANGE:` block at the very bottom.
5. Output **only** the raw commit message — no introductory or concluding text.
# Authentication

The WhatsPro.Net SDK is designed to make authentication as painless as possible. The `WhatsProHttpClient` manages your JSON Web Tokens (JWT) automatically, ensuring that you never have to manually attach `Bearer` headers or worry about token expiration during a long-running process.

## How it works

When you instantiate a `WhatsProClient` with your `Email` and `Password` inside `WhatsProOptions`, it doesn't immediately log in. Instead, it waits until you make your first authenticated API call. 

Before making that call, the SDK checks if it has a valid token. If not, it silently performs a login request, caches the token, and then proceeds with your requested operation. If the token expires later, the SDK detects the 401 Unauthorized response, automatically logs in again to get a fresh token, and replays your original request.

---

## Explicit Login & Profile Retrieval

If you want to eagerly authenticate and verify credentials on startup, you can call the profile endpoint.

```csharp
try
{
    // Getting the profile will implicitly trigger a login if there's no active token
    var profileResponse = await client.Auth.GetProfileAsync();
    
    Console.WriteLine($"Successfully connected!");
    Console.WriteLine($"Welcome, {profileResponse.Data.User.Name}!");
    Console.WriteLine($"Your Plan: {profileResponse.Data.Plan.Name}");
}
catch (Exception ex)
{
    Console.WriteLine("Failed to authenticate. Please check your credentials.");
}
```

## Managing Your Account

The `AuthOperations` class provides methods for account management, allowing you to build a self-service portal if you are wrapping this SDK for your own users.

### Updating Profile Details

```csharp
var updateReq = new WhatsPro.Authentication.Models.UpdateProfileRequest
{
    Name = "My New Company Name",
    Phone = "+19876543210"
};

await client.Auth.UpdateProfileAsync(updateReq);
```

### Changing Password

```csharp
var passwordReq = new WhatsPro.Authentication.Models.ChangePasswordRequest
{
    CurrentPassword = "old_password_123",
    Password = "new_secure_password_456",
    PasswordConfirmation = "new_secure_password_456"
};

await client.Auth.ChangePasswordAsync(passwordReq);
```

## The Static API Token

WhatsPro offers two ways to send messages: an **Encrypted** endpoint (requires JWT and session keys) and an **Unencrypted Token-based** endpoint (simpler, uses a static token).

If you want to use the simpler unencrypted endpoint from another lightweight script or system, you can retrieve your account's static API token using the SDK:

```csharp
string myStaticToken = await client.Auth.GetApiTokenAsync();
Console.WriteLine($"My API Token is: {myStaticToken}");
```

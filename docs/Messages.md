# Sending Messages

Sending messages is the core functionality of WhatsPro. The SDK provides two distinct ways to send messages, depending on your security requirements and infrastructure.

## 1. Encrypted Messages (Recommended)

The encrypted messaging endpoint provides end-to-end payload encryption between your .NET application and the WhatsPro server. When you use `client.Messages.SendAsync`, the SDK automatically:
1. Fetches the cryptographic keys for your connected WhatsApp session.
2. AES-encrypts your message payload.
3. Sends the cipher data securely over the wire.

Use this method when sending sensitive information like OTPs, financial alerts, or personal data.

```csharp
var request = new WhatsPro.Messages.Models.SendMessageRequest
{
    SendPhone = true, // Set to true to send to a direct phone number
    Phones = new[] { "+1234567890" },
    Message = "Hello from the secure encrypted endpoint! 🔒"
};

var response = await client.Messages.SendAsync(request);
Console.WriteLine(response.Message);
```

## 2. Unencrypted Token Messages (Fallback)

If you have issues with the encryption keys or prefer the simpler legacy endpoint, you can use `SendNonEncryptedAsync`. This endpoint does not require AES payload encryption and instead relies on standard HTTPS and your account's static API Token (which the SDK injects automatically).

```csharp
var unencryptedRequest = new WhatsPro.Messages.Models.SendMessageRequest
{
    SendPhone = true,
    Phones = new[] { "+1234567890" },
    Message = "Hello from the unencrypted endpoint!",
    ImgUrl = "https://example.com/promotional-banner.png" // You can attach media URLs
};

await client.Messages.SendNonEncryptedAsync(unencryptedRequest);
```

---

## Managing Message History

You can view the history of messages you've sent, which is useful for building audit logs or delivery receipt dashboards.

### Listing Sent Messages

```csharp
var pagination = new WhatsPro.Models.PaginationRequest
{
    Page = 1,
    Limit = 100
};

var history = await client.Messages.ListAsync(pagination);

foreach (var msg in history.Data.Data)
{
    Console.WriteLine($"Sent to: {msg.Receiver}, Status: {msg.Status}");
}
```

### Deleting Messages from History

If you need to comply with GDPR or simply clean up your logs, you can delete message records.

```csharp
var deleteReq = new WhatsPro.Models.DeleteRequest
{
    Ids = new List<int> { 54321, 54322 } // IDs of the message records
};

await client.Messages.DeleteAsync(deleteReq);
```

## Uploading Documents

If you need to send a local file (like a generated PDF invoice) instead of a public URL, you must first upload the document to WhatsPro.

```csharp
using var fileStream = System.IO.File.OpenRead("invoice-1024.pdf");

// Upload the file
var uploadResult = await client.Messages.UploadDocumentAsync("invoice-1024.pdf", fileStream);

// Send the uploaded file URL in your message
var request = new WhatsPro.Messages.Models.SendMessageRequest
{
    SendPhone = true,
    Phones = new[] { "+1234567890" },
    Message = "Here is your invoice.",
    FileUrl = uploadResult.Data.Url // Use the URL returned from the upload
};

await client.Messages.SendNonEncryptedAsync(request);
```

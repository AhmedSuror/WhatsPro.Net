# WhatsApp Sessions

The **Sessions** module allows you to manage the actual WhatsApp accounts connected to your WhatsPro workspace. Each session represents a physical WhatsApp account linked via a QR code.

## Why manage sessions via API?

Normally, you link a WhatsApp account via the WhatsPro web dashboard. However, you might want to:
* Build your own white-labeled dashboard.
* Disconnect numbers programmatically if an employee leaves.
* Set up webhooks programmatically so your system receives incoming messages.

---

## Listing Connected Sessions

You can retrieve a list of all WhatsApp accounts connected to your workspace.

```csharp
var request = new WhatsPro.Models.PaginationRequest { Page = 1, Limit = 10 };
var response = await client.Sessions.ListAsync(request);

foreach (var session in response.Data.Data)
{
    Console.WriteLine($"Session Name: {session.Name}, Phone: {session.Phone}, Status: {session.Status}");
}
```

## Connecting and QR Codes

If a session is disconnected, you can request a connection. If the session needs a QR code scan, the API will return the QR code data which you can render in your own UI.

```csharp
int sessionId = 10;
var connectResponse = await client.Sessions.ConnectAsync(sessionId);

if (connectResponse.Data.Status == "qr")
{
    Console.WriteLine("Please scan this QR code to connect:");
    Console.WriteLine(connectResponse.Data.QrCode); // Render this base64 image or string in your UI
}
else if (connectResponse.Data.Status == "connected")
{
    Console.WriteLine("Session is successfully connected!");
}
```

## Setting up Webhooks

To build interactive chatbots or receive delivery reports, you need to configure a webhook URL. WhatsPro will POST JSON events to this URL when messages are received.

```csharp
var webhookReq = new WhatsPro.Sessions.Models.SetWebhookRequest
{
    WebhookUrl = "https://your-api.com/api/whatspro-webhook",
    WebhookEvents = new List<string> { "message_received", "message_status_updated" }
};

await client.Sessions.SetWebhookAsync(sessionId, webhookReq);
Console.WriteLine("Webhook configured successfully.");
```

## Disconnecting a Session

You can temporarily disconnect a session or permanently delete it.

```csharp
// Temporarily stop the session (keeps credentials)
await client.Sessions.DisconnectAsync(sessionId, forever: false);

// Permanently logout and delete credentials
await client.Sessions.DisconnectAsync(sessionId, forever: true);
```

# Managing Clients

The **Clients** module in WhatsPro.Net allows you to programmatically manage your CRM contacts within the WhatsPro system. You can retrieve clients, create new ones, manage their associated phone numbers, move them between groups, and even import large sets of clients from Excel files.

## When to use this module?

You should use `ClientOperations` when you want to sync your external database (like a CRM or ERP) with WhatsPro. For example, when a new user registers on your website, you can automatically create them as a client in WhatsPro so they are ready for future broadcast campaigns.

---

## Retrieving Clients

If you need to build a UI displaying all contacts or run a background sync task, you can retrieve a paginated list of clients.

```csharp
var request = new WhatsPro.Models.PaginationRequest
{
    Page = 1,
    Limit = 50, // Get 50 clients per page
    Search = "John" // Optional: filter by name
};

var response = await client.Clients.ListAsync(request);

Console.WriteLine($"Total clients: {response.Data.Total}");
foreach (var clientInfo in response.Data.Data)
{
    Console.WriteLine($"Name: {clientInfo.Name}, Primary Phone: {clientInfo.Phone}");
}
```

## Creating a New Client

To add a new contact to WhatsPro, use `CreateAsync`. This is perfect for hooking into "User Registered" events in your application.

```csharp
var createReq = new WhatsPro.Clients.Models.CreateClientRequest
{
    Name = "John Doe",
    Phone = "01010959716",
    GroupId = 1 // The ID of the group to add this client to
};

var createRes = await client.Clients.CreateAsync(createReq);
if (createRes.Status)
{
    Console.WriteLine("Client created successfully!");
}
```

## Adding Additional Phones

A single client can have multiple phone numbers. This is useful for users who have a personal and a business WhatsApp number.

```csharp
var addPhoneReq = new WhatsPro.Clients.Models.AddPhoneRequest
{
    ClientId = 42, // The ID of the client you just created
    Phones = new List<string> { "+1234567890", "+0987654321" }
};

await client.Clients.AddPhoneAsync(addPhoneReq);
```

## Changing a Client's Group

If a user upgrades their subscription plan on your site, you might want to move them from a "Free Tier" group to a "Premium Users" group in WhatsPro to send them exclusive broadcasts.

```csharp
var changeGroupReq = new WhatsPro.Clients.Models.ChangeGroupRequest
{
    ClientId = 42,
    GroupId = 5 // ID of the Premium Users group
};

await client.Clients.ChangeGroupAsync(changeGroupReq);
```

## Bulk Importing from Excel

When migrating from another system, creating clients one-by-one can be slow and hit rate limits. Use the bulk import feature to upload an Excel file directly.

```csharp
using var fileStream = System.IO.File.OpenRead("path/to/your/clients_export.xlsx");

// The SDK handles the complex multipart-form data encoding automatically!
var importResponse = await client.Clients.ImportFromExcelAsync(fileStream);

Console.WriteLine($"Import Result: {importResponse.Message}");
```

# Managing Groups

The **Groups** module allows you to organize your clients into targetable segments. A Group in WhatsPro acts as a label or a list (e.g., "VIP Customers", "Newsletter Subscribers", "Event Attendees").

## Why use Groups?

Organizing clients into groups is critical for broadcast messaging. Instead of sending a message to 1,000 individual phone numbers, you can tell WhatsPro to send a campaign to a specific Group ID.

---

## Listing and Retrieving Groups

You can retrieve groups with pagination, or if you just need a quick list for a dropdown menu in your application, you can grab all of them at once.

### Getting All Groups (No Pagination)
This is the easiest way to populate a select box in your UI.
```csharp
var response = await client.Groups.GetAllAsync();

foreach (var group in response.Data)
{
    Console.WriteLine($"Group ID: {group.Id}, Name: {group.Name}");
}
```

### Paginated List
If you have hundreds of groups and need a data grid, use `ListAsync`.
```csharp
var request = new WhatsPro.Models.PaginationRequest
{
    Page = 1,
    Limit = 10
};
var pagedResponse = await client.Groups.ListAsync(request);
```

## Creating a Group

When a new event is created in your system, you can automatically create a corresponding group in WhatsPro.

```csharp
var createReq = new WhatsPro.Groups.Models.CreateGroupRequest
{
    Name = "Summer Sale 2026",
    Notes = "Created automatically"
};

var createRes = await client.Groups.CreateAsync(createReq);
Console.WriteLine($"Group Created with ID: {createRes.Data.Id}");
```

## Managing Group Members (Clients)

Sometimes you need to do bulk operations on the clients within a group, like moving them to a new list after a campaign ends.

### Transferring Clients
Move selected clients (or all clients) from one group to another.

```csharp
var transferReq = new WhatsPro.Groups.Models.TransferClientsRequest
{
    Ids = new List<int> { 101, 102, 103 }, // Source Client IDs or Group IDs
    GroupId = 2 // Destination Group ID
};

await client.Groups.TransferClientsAsync(transferReq);
```

### Deleting Clients
Remove clients from a group in bulk.

```csharp
var deleteReq = new WhatsPro.Groups.Models.DeleteGroupClientsRequest
{
    Ids = new List<int> { 101, 102, 103 } // IDs of the groups to destroy clients from
};

await client.Groups.DeleteClientsAsync(deleteReq);
```

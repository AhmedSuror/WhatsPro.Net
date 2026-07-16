# Dashboard Analytics

The **Dashboard** module allows you to retrieve real-time analytics and statistics about your WhatsPro workspace. This is incredibly useful if you want to embed WhatsPro metrics directly into your own internal admin panel or CRM dashboard.

## Retrieving Overview Statistics

You can fetch the high-level metrics of your account, such as total messages sent, success rates, and active sessions.

```csharp
var response = await client.Dashboard.GetDashboardAsync();

Console.WriteLine($"Total Sent Messages: {response.Data.TotalSent}");
Console.WriteLine($"Total Failed Messages: {response.Data.TotalFailed}");
```

## Retrieving Send Charts

If you want to render a line chart or bar chart in your frontend showing message volume over time, use the `GetSendChartAsync` method.

```csharp
var chartRequest = new WhatsPro.Dashboard.Models.SendChartRequest
{
    Days = 7 // Fetch data for the last 7 days
};

var chartResponse = await client.Dashboard.GetSendChartAsync(chartRequest);

Console.WriteLine("Messages sent per day for the last 7 days:");
foreach (var count in chartResponse.Data)
{
    Console.WriteLine($"- {count}");
}
```

## Getting Top Engaged Numbers

You can fetch a list of phone numbers that have sent or received the most messages. This is great for identifying your most active customers or chatty users.

```csharp
int topN = 5; // Get top 5 numbers
var topNumbersResponse = await client.Dashboard.GetTopNumbersAsync(topN);

Console.WriteLine($"Top {topN} Numbers:");
foreach (var item in topNumbersResponse.Data)
{
    Console.WriteLine($"{item.Phone}: {item.MessageCount} messages");
}
```

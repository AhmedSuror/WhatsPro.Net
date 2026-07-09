using System;
using System.Threading.Tasks;
using WhatsPro;
using WhatsPro.Models;

namespace WhatsPro.ConsoleSample;

class Program
{
    static async Task Main(string[] args)
    {
        // Load user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        var secretEmail = configuration["User:Email"];
        var secretPassword = configuration["User:Password"];

        Console.WriteLine("WhatsPro.Net Interactive Sample");
        Console.Write("Enter BaseUrl (e.g. https://whats-pro.net/backend/public/index.php/api): ");
        var baseUrl = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(baseUrl))
            baseUrl = "https://whats-pro.net/backend/public/index.php/api";
        
        // Email — use secret as default if available
        string email;
        if (!string.IsNullOrWhiteSpace(secretEmail))
        {
            Console.Write($"Enter Email (press Enter to use '{secretEmail}'): ");
            var input = Console.ReadLine();
            email = string.IsNullOrWhiteSpace(input) ? secretEmail! : input!;
        }
        else
        {
        Console.Write("Enter Email: ");
            email = Console.ReadLine()!;
        }

        // Password — use secret as default if available
        string password;
        if (!string.IsNullOrWhiteSpace(secretPassword))
        {
            Console.Write("Enter Password (press Enter to use saved secret): ");
            var input = Console.ReadLine();
            password = string.IsNullOrWhiteSpace(input) ? secretPassword! : input!;
        }
        else
        {
        Console.Write("Enter Password: ");
            password = Console.ReadLine()!;
        }

        Console.WriteLine("─────────────────────────────────────────────");

        var options = new WhatsProOptions
        {
            BaseUrl = baseUrl,
            Email = email!,
            Password = password!
        };

        using var client = new WhatsProClient(options);

        while (true)
        {
            Console.WriteLine("\n--- Menu ---");
            Console.WriteLine("1. Get Profile (Login implicitly)");
            Console.WriteLine("2. List Clients");
            Console.WriteLine("3. Get Dashboard");
            Console.WriteLine("4. Send Message");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        var profile = await client.Auth.GetProfileAsync();
                        Console.WriteLine($"Success: Logged in as {profile.Data.User.Name} (ID: {profile.Data.User.Id})");
                        break;
                    case "2":
                        var clients = await client.Clients.ListAsync(new PaginationRequest());
                        Console.WriteLine($"Success: Found {clients.Data.Total} total clients.");
                        foreach (var c in clients.Data.Data)
                        {
                            Console.WriteLine($" - {c.Name} ({c.Phone})");
                        }
                        break;
                    case "3":
                        var dashboard = await client.Dashboard.GetDashboardAsync();
                        Console.WriteLine($"Success: Dashboard loaded. Number of cards: {dashboard.Data.Cards.Count}");
                        break;
                    case "4":
                        Console.Write("Enter target phone number (e.g. 2010...): ");
                        var targetPhone = Console.ReadLine();
                        Console.Write("Enter message text: ");
                        var msgText = Console.ReadLine();
                        var msgRequest = new Messages.Models.SendMessageRequest
                        {
                            SendPhone = targetPhone!,
                            Message = msgText!
                        };
                        var sendResult = await client.Messages.SendAsync(msgRequest);
                        Console.WriteLine($"Success: Message sent. API says: {sendResult.Message}");
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}

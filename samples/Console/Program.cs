using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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

        // Enable UTF-8 output so Arabic and other Unicode text renders correctly
        Console.OutputEncoding = Encoding.UTF8;

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
            BaseUrl = baseUrl!,
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
            Console.WriteLine("4. Send Message (Encrypted)");
            Console.WriteLine("5. Send Message (Non-Encrypted)");
            Console.WriteLine("6. View API Token");
            Console.WriteLine("7. Sessions Menu");
            Console.WriteLine("8. Exit");
            Console.Write("Choose an option: ");
            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        var profile = await client.Auth.GetProfileAsync();
                        if (profile.Data != null)
                            ConsoleDisplayHelper.PrintProfile(profile.Data);
                        else
                            Console.WriteLine("Profile retrieved but user data was unavailable.");
                        break;
                    case "2":
                        var clients = await client.Clients.ListAsync(new PaginationRequest());
                        ConsoleDisplayHelper.PrintClients(clients.Data);
                        break;
                    case "3":
                        var dashboard = await client.Dashboard.GetDashboardAsync();
                        ConsoleDisplayHelper.PrintDashboard(dashboard.Data);
                        break;
                    case "4":
                        Console.Write("Enter target phone number (e.g. 2010...): ");
                        var targetPhone = Console.ReadLine();
                        Console.Write("Enter message text: ");
                        var msgText = Console.ReadLine();
                        var msgRequest = new Messages.Models.SendMessageRequest
                        {
                            SendPhone = true,
                            Phones = new System.Collections.Generic.List<string> { targetPhone! },
                            Message = msgText!
                        };
                        var sendResult = await client.Messages.SendAsync(msgRequest);
                        Console.WriteLine($"Success: Message sent. API says: {sendResult.Message}");
                        break;
                    case "5":
                        Console.WriteLine("\n--- Send Non-Encrypted Message ---");
                        Console.WriteLine("1. Full phone format");
                        Console.WriteLine("2. Country code");
                        Console.WriteLine("3. Image URL");
                        Console.WriteLine("4. Base64 Image");
                        Console.WriteLine("5. Document");
                        Console.Write("Choose type: ");
                        var typeChoice = Console.ReadLine();
                        
                        var msgRequestNon = new Messages.Models.SendMessageRequest { SendPhone = true };

                        if (typeChoice == "1" || typeChoice == "2" || typeChoice == "3" || typeChoice == "4" || typeChoice == "5")
                        {
                            Console.Write("Enter target phone number: ");
                            var targetPhoneNon = Console.ReadLine();
                            msgRequestNon.Phones = new System.Collections.Generic.List<string> { targetPhoneNon! };
                            
                            Console.Write("Enter message text: ");
                            msgRequestNon.Message = Console.ReadLine()!;
                            
                            if (typeChoice == "2" || typeChoice == "3" || typeChoice == "4")
                            {
                                Console.Write("Enter country code (e.g. EG): ");
                                msgRequestNon.CountryCode = Console.ReadLine()!;
                            }
                            
                            if (typeChoice == "3")
                            {
                                Console.Write("Enter Image URL: ");
                                msgRequestNon.ImgUrl = Console.ReadLine()!;
                            }
                            else if (typeChoice == "4")
                            {
                                Console.Write("Enter Base64 Image data: ");
                                msgRequestNon.Img = Console.ReadLine()!;
                            }
                            else if (typeChoice == "5")
                            {
                                Console.Write("Enter file path to upload: ");
                                var filePath = Console.ReadLine()?.Trim('"');
                                if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
                                {
                                    Console.WriteLine($"File not found: {filePath}");
                                    break;
                                }
                                Console.WriteLine("Uploading document...");
                                using var stream = System.IO.File.OpenRead(filePath);
                                var fileName = System.IO.Path.GetFileName(filePath);
                                var uploadResult = await client.Messages.UploadDocumentAsync(fileName, stream);
                                if (uploadResult.Success && uploadResult.Data != null)
                                {
                                    msgRequestNon.DocId = uploadResult.Data.Id;
                                    Console.WriteLine($"Document uploaded. ID: {msgRequestNon.DocId}");
                                }
                                else
                                {
                                    Console.WriteLine($"Upload failed: {uploadResult.Message}");
                                    break;
                                }
                            }
                            
                            var sendResultNon = await client.Messages.SendNonEncryptedAsync(msgRequestNon);
                            Console.WriteLine($"Success: Message sent (Non-Encrypted). API says: {sendResultNon.Message}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid type choice.");
                        }
                        break;
                    case "6":
                        var token = await client.Auth.GetApiTokenAsync();
                        Console.WriteLine($"Your API Token is: {token}");
                        break;
                    case "7":
                        Console.WriteLine("\n--- Sessions Menu ---");
                        Console.WriteLine("1. List Sessions");
                        Console.WriteLine("2. Get Session by ID");
                        Console.WriteLine("3. Connect Session");
                        Console.WriteLine("4. Disconnect Session");
                        Console.WriteLine("5. Change Session Name");
                        Console.WriteLine("6. Set Webhook URL");
                        Console.Write("Choose an option: ");
                        var sessionChoice = Console.ReadLine();
                        switch (sessionChoice)
                        {
                            case "1":
                                var sessions = await client.Sessions.ListAsync(new PaginationRequest());
                                ConsoleDisplayHelper.PrintSessions(sessions.Data);
                                break;
                            case "2":
                                Console.Write("Enter Session ID: ");
                                if (int.TryParse(Console.ReadLine(), out int sessionId))
                                {
                                    var session = await client.Sessions.GetAsync(sessionId);
                                    if (session != null && session.Data != null)
                                        Console.WriteLine($"Session: {session.Data.Name} - Status: {session.Data.Status}");
                                    else
                                        Console.WriteLine($"Failed to get session: {session?.Message ?? "No response"}");
                                }
                                else { Console.WriteLine("Invalid Session ID."); }
                                break;
                            case "3":
                                Console.Write("Enter Session ID to Connect: ");
                                if (int.TryParse(Console.ReadLine(), out int cId))
                                {
                                    var result = await client.Sessions.ConnectAsync(cId);
                                    if (result != null)
                                    {
                                        Console.WriteLine($"API says: {result.Message}");
                                        if (result.Data != null && !string.IsNullOrEmpty(result.Data.Qr))
                                        {
                                            if (result.Data.Qr == "connected" || result.Data.Qr == "qr")
                                            {
                                                Console.WriteLine($"Status: {result.Data.Qr}");
                                            }
                                            else
                                            {
                                                Console.WriteLine($"QR Code received. Attempting to open...");
                                                try
                                                {
                                                    string base64 = result.Data.Qr.Replace("data:image/png;base64,", "");
                                                    byte[] imageBytes = Convert.FromBase64String(base64);
                                                    string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"qr_{cId}.png");
                                                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                                                    Console.WriteLine($"Opened QR Code from {filePath}");
                                                }
                                                catch (Exception ex)
                                                {
                                                    Console.WriteLine($"Failed to open QR Code: {ex.Message}");
                                                    Console.WriteLine($"QR Code data length: {result.Data.Qr.Length}");
                                                }
                                            }
                                        }
                                    }
                                    else { Console.WriteLine("API returned null response."); }
                                }
                                else { Console.WriteLine("Invalid Session ID."); }
                                break;
                            case "4":
                                Console.Write("Enter Session ID to Disconnect: ");
                                if (int.TryParse(Console.ReadLine(), out int dId))
                                {
                                    Console.Write("Disconnect forever? (y/n): ");
                                    bool forever = Console.ReadLine()?.Trim().ToLower() == "y";
                                    var result = await client.Sessions.DisconnectAsync(dId, forever);
                                    Console.WriteLine($"API says: {result?.Message ?? "Null response"}");
                                }
                                else { Console.WriteLine("Invalid Session ID."); }
                                break;
                            case "5":
                                Console.Write("Enter Session ID to Change Name: ");
                                if (int.TryParse(Console.ReadLine(), out int cnId))
                                {
                                    var result = await client.Sessions.ChangeNameAsync(cnId);
                                    Console.WriteLine($"API says: {result?.Message ?? "Null response"}");
                                }
                                else { Console.WriteLine("Invalid Session ID."); }
                                break;
                            case "6":
                                Console.Write("Enter Session ID to Set Webhook: ");
                                if (int.TryParse(Console.ReadLine(), out int wId))
                                {
                                    Console.Write("Enter new Webhook URL: ");
                                    var wUrl = Console.ReadLine();
                                    var result = await client.Sessions.SetWebhookAsync(wId, new WhatsPro.Sessions.Models.SetWebhookRequest { Url = wUrl ?? string.Empty });
                                    Console.WriteLine($"API says: {result?.Message ?? "Null response (Silent fail)"}");
                                }
                                else { Console.WriteLine("Invalid Session ID."); }
                                break;
                            default:
                                Console.WriteLine("Invalid session option.");
                                break;
                        }
                        break;
                    case "8":
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

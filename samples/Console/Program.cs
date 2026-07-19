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
            Console.WriteLine("2. Groups Menu");
            Console.WriteLine("3. Clients Menu");
            Console.WriteLine("4. Dashboard Menu");
            Console.WriteLine("5. Manage Messages (Encrypted)");
            Console.WriteLine("6. Send Message (Non-Encrypted)");
            Console.WriteLine("7. View API Token");
            Console.WriteLine("8. Sessions Menu");
            Console.WriteLine("9. Exit");
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
                        Console.WriteLine("\n--- Groups Menu ---");
                        Console.WriteLine("1. List Groups");
                        Console.WriteLine("2. Get Group by ID");
                        Console.WriteLine("3. Get All Groups");
                        Console.WriteLine("4. Create Group");
                        Console.WriteLine("5. Update Group");
                        Console.WriteLine("6. Delete Group(s)");
                        Console.WriteLine("7. Transfer Clients to Another Group");
                        Console.WriteLine("8. Delete Group's Clients");
                        Console.Write("Choose an option: ");
                        var groupChoice = Console.ReadLine();
                        switch (groupChoice)
                        {
                            case "1":
                                Console.Write("Enter page number (default 1): ");
                                var pageInput = Console.ReadLine();
                                var pageNum = int.TryParse(pageInput, out int p) ? p : 1;
                                var groups = await client.Groups.ListAsync(new PaginationRequest { Page = pageNum });
                                if (groups.Data != null)
                                    ConsoleDisplayHelper.PrintGroups(groups.Data);
                                else
                                    Console.WriteLine("No groups returned.");
                                break;
                            case "2":
                                Console.Write("Enter Group ID: ");
                                if (int.TryParse(Console.ReadLine(), out int groupId))
                                {
                                    var groupInfo = await client.Groups.GetAsync(groupId);
                                    if (groupInfo.Data != null)
                                        ConsoleDisplayHelper.PrintGroupDetail(groupInfo.Data);
                                    else
                                        Console.WriteLine($"No group found with ID {groupId}.");
                                }
                                else { Console.WriteLine("Invalid Group ID."); }
                                break;
                            case "3":
                                var allGroups = await client.Groups.GetAllAsync();
                                if (allGroups.Data != null)
                                {
                                    var dummyPaged = new PagedResponse<WhatsPro.Groups.Models.GroupInfo>
                                    {
                                        Data = allGroups.Data,
                                        CurrentPage = 1,
                                        LastPage = 1,
                                        Total = allGroups.Data.Count
                                    };
                                    ConsoleDisplayHelper.PrintGroups(dummyPaged);
                                }
                                else
                                {
                                    Console.WriteLine("No groups returned.");
                                }
                                break;
                            case "4":
                                Console.Write("Enter Group Name: ");
                                var cName = Console.ReadLine() ?? "";
                                Console.Write("Enter Notes (optional): ");
                                var cNotes = Console.ReadLine();
                                
                                var createReq = new WhatsPro.Groups.Models.CreateGroupRequest
                                {
                                    Name = cName,
                                    Notes = cNotes
                                };
                                var createRes = await client.Groups.CreateAsync(createReq);
                                Console.WriteLine($"API says: {createRes.Message}");
                                break;
                            case "5":
                                Console.Write("Enter Group ID to Update: ");
                                if (int.TryParse(Console.ReadLine(), out int uId))
                                {
                                    Console.Write("Enter Name: ");
                                    var uName = Console.ReadLine() ?? "";
                                    Console.Write("Enter Notes (optional): ");
                                    var uNotes = Console.ReadLine();

                                    var updateReq = new WhatsPro.Groups.Models.UpdateGroupRequest
                                    {
                                        Name = uName,
                                        Notes = uNotes
                                    };
                                    var updateRes = await client.Groups.UpdateAsync(uId, updateReq);
                                    Console.WriteLine($"API says: {updateRes.Message}");
                                }
                                else { Console.WriteLine("Invalid Group ID."); }
                                break;
                            case "6":
                                Console.Write("Enter Group ID(s) to delete (comma-separated): ");
                                var dIdsInput = Console.ReadLine();
                                var deleteIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (dIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int did))
                                        deleteIds.Add(did);
                                }
                                if (deleteIds.Count == 0) { Console.WriteLine("No valid IDs provided."); break; }
                                var deleteRes = await client.Groups.DeleteAsync(new WhatsPro.Models.DeleteRequest { Ids = deleteIds });
                                Console.WriteLine($"API says: {deleteRes.Message}");
                                break;
                            case "7":
                                Console.Write("Enter Group ID(s) whose clients to transfer (comma-separated): ");
                                var tIdsInput = Console.ReadLine();
                                var transferIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (tIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int tid))
                                        transferIds.Add(tid);
                                }
                                if (transferIds.Count == 0) { Console.WriteLine("No valid source Group IDs provided."); break; }
                                Console.Write("Enter Target Group ID: ");
                                if (int.TryParse(Console.ReadLine(), out int tGroupId))
                                {
                                    var transferReq = new WhatsPro.Groups.Models.TransferClientsRequest
                                    {
                                        Ids = transferIds,
                                        GroupId = tGroupId
                                    };
                                    var transferRes = await client.Groups.TransferClientsAsync(transferReq);
                                    Console.WriteLine($"API says: {transferRes.Message}");
                                }
                                else { Console.WriteLine("Invalid Target Group ID."); }
                                break;
                            case "8":
                                Console.Write("Enter Group ID(s) to delete clients from (comma-separated): ");
                                var dcIdsInput = Console.ReadLine();
                                var deleteClientIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (dcIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int dcid))
                                        deleteClientIds.Add(dcid);
                                }
                                if (deleteClientIds.Count == 0) { Console.WriteLine("No valid Group IDs provided."); break; }
                                var deleteClientsReq = new WhatsPro.Groups.Models.DeleteGroupClientsRequest { Ids = deleteClientIds };
                                var deleteClientsRes = await client.Groups.DeleteClientsAsync(deleteClientsReq);
                                Console.WriteLine($"API says: {deleteClientsRes.Message}");
                                break;
                            default:
                                Console.WriteLine("Invalid group option.");
                                break;
                        }
                        break;

                    case "3":
                        Console.WriteLine("\n--- Clients Menu ---");
                        Console.WriteLine("1. List Clients");
                        Console.WriteLine("2. Get Client by ID");
                        Console.WriteLine("3. Create Client");
                        Console.WriteLine("4. Update Client");
                        Console.WriteLine("5. Delete Client(s)");
                        Console.WriteLine("6. Add Extra Phone");
                        Console.WriteLine("7. Change Group");
                        Console.WriteLine("8. Import From Excel");
                        Console.Write("Choose an option: ");
                        var clientChoice = Console.ReadLine();
                        switch (clientChoice)
                        {
                            case "1":
                                Console.Write("Enter page number (default 1): ");
                                var pageInput = Console.ReadLine();
                                var pageNum = int.TryParse(pageInput, out int p) ? p : 1;
                                var clients = await client.Clients.ListAsync(new PaginationRequest { Page = pageNum });
                                if (clients.Data != null)
                                    ConsoleDisplayHelper.PrintClients(clients.Data);
                                else
                                    Console.WriteLine("No clients returned.");
                                break;
                            case "2":
                                Console.Write("Enter Client ID: ");
                                if (int.TryParse(Console.ReadLine(), out int clientId))
                                {
                                    var clientInfo = await client.Clients.GetAsync(clientId);
                                    if (clientInfo.Data != null)
                                        ConsoleDisplayHelper.PrintClientDetail(clientInfo.Data);
                                    else
                                        Console.WriteLine($"No client found with ID {clientId}.");
                                }
                                else { Console.WriteLine("Invalid Client ID."); }
                                break;
                            case "3":
                                Console.Write("Enter Phone (e.g. +2010...): ");
                                var cPhone = Console.ReadLine() ?? "";
                                Console.Write("Enter Name: ");
                                var cName = Console.ReadLine() ?? "";
                                Console.Write("Enter Group ID: ");
                                int cGroupId = int.TryParse(Console.ReadLine(), out int cgid) ? cgid : 0;
                                Console.Write("Enter Notes (optional): ");
                                var cNotes = Console.ReadLine();
                                
                                var createReq = new WhatsPro.Clients.Models.CreateClientRequest
                                {
                                    Phone = cPhone,
                                    Name = cName,
                                    GroupId = cGroupId,
                                    Notes = cNotes
                                };
                                var createRes = await client.Clients.CreateAsync(createReq);
                                Console.WriteLine($"API says: {createRes.Message}");
                                break;
                            case "4":
                                Console.Write("Enter Client ID to Update: ");
                                if (int.TryParse(Console.ReadLine(), out int uId))
                                {
                                    Console.Write("Enter Name: ");
                                    var uName = Console.ReadLine() ?? "";
                                    Console.Write("Enter Phone: ");
                                    var uPhone = Console.ReadLine() ?? "";
                                    Console.Write("Enter Group ID: ");
                                    int uGroupId = int.TryParse(Console.ReadLine(), out int ugid) ? ugid : 0;
                                    Console.Write("Enter Notes (optional): ");
                                    var uNotes = Console.ReadLine();

                                    var updateReq = new WhatsPro.Clients.Models.UpdateClientRequest
                                    {
                                        Name = uName,
                                        Phone = uPhone,
                                        GroupId = uGroupId,
                                        Notes = uNotes
                                    };
                                    var updateRes = await client.Clients.UpdateAsync(uId, updateReq);
                                    Console.WriteLine($"API says: {updateRes.Message}");
                                }
                                else { Console.WriteLine("Invalid Client ID."); }
                                break;
                            case "5":
                                Console.Write("Enter Client ID(s) to delete (comma-separated): ");
                                var dIdsInput = Console.ReadLine();
                                var deleteClientIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (dIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int did))
                                        deleteClientIds.Add(did);
                                }
                                if (deleteClientIds.Count == 0) { Console.WriteLine("No valid IDs provided."); break; }
                                var deleteRes = await client.Clients.DeleteAsync(new WhatsPro.Models.DeleteRequest { Ids = deleteClientIds });
                                Console.WriteLine($"API says: {deleteRes.Message}");
                                break;
                            case "6":
                                Console.Write("Enter Client ID to add phone to: ");
                                if (int.TryParse(Console.ReadLine(), out int pClientId))
                                {
                                    Console.Write("Enter Extra Phone Number: ");
                                    var pPhone = Console.ReadLine() ?? "";
                                    var addPhoneReq = new WhatsPro.Clients.Models.AddPhoneRequest
                                    {
                                        ClientId = pClientId,
                                        Phone = pPhone
                                    };
                                    var addPhoneRes = await client.Clients.AddPhoneAsync(addPhoneReq);
                                    Console.WriteLine($"API says: {addPhoneRes.Message}");
                                }
                                else { Console.WriteLine("Invalid Client ID."); }
                                break;
                            case "7":
                                Console.Write("Enter Client ID(s) to change group (comma-separated): ");
                                var cIdsInput = Console.ReadLine();
                                var changeIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (cIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int cid))
                                        changeIds.Add(cid);
                                }
                                if (changeIds.Count == 0) { Console.WriteLine("No valid IDs provided."); break; }
                                Console.Write("Enter Target Group ID: ");
                                if (int.TryParse(Console.ReadLine(), out int tGroupId))
                                {
                                    var changeGroupReq = new WhatsPro.Clients.Models.ChangeGroupRequest
                                    {
                                        Ids = changeIds,
                                        GroupId = tGroupId
                                    };
                                    var changeRes = await client.Clients.ChangeGroupAsync(changeGroupReq);
                                    Console.WriteLine($"API says: {changeRes.Message}");
                                }
                                else { Console.WriteLine("Invalid Target Group ID."); }
                                break;
                            case "8":
                                Console.Write("Enter path to Excel file: ");
                                var excelPath = Console.ReadLine()?.Trim('"');
                                if (string.IsNullOrWhiteSpace(excelPath) || !System.IO.File.Exists(excelPath))
                                {
                                    Console.WriteLine($"File not found: {excelPath}");
                                    break;
                                }
                                Console.WriteLine("Uploading Excel file...");
                                using (var stream = System.IO.File.OpenRead(excelPath))
                                {
                                    var importRes = await client.Clients.ImportFromExcelAsync(stream);
                                    Console.WriteLine($"API says: {importRes.Message}");
                                }
                                break;
                            default:
                                Console.WriteLine("Invalid client option.");
                                break;
                        }
                        break;
                    case "4":
                        Console.WriteLine("\n--- Dashboard Menu ---");
                        Console.WriteLine("1. Dashboard Summary");
                        Console.WriteLine("2. Send Chart");
                        Console.WriteLine("3. Top Numbers");
                        Console.Write("Choose an option: ");
                        var dashChoice = Console.ReadLine();
                        switch (dashChoice)
                        {
                            case "1":
                                var dashboard = await client.Dashboard.GetDashboardAsync();
                                ConsoleDisplayHelper.PrintDashboard(dashboard.Data);
                                break;
                            case "2":
                                var currentYear = DateTime.Now.Year;
                                var currentMonth = DateTime.Now.Month;
                                var chartReq = new WhatsPro.Dashboard.Models.SendChartRequest
                                {
                                    Type = "month",
                                    Year = currentYear,
                                    Month = currentMonth
                                };
                                var chart = await client.Dashboard.GetSendChartAsync(chartReq);
                                ConsoleDisplayHelper.PrintSendChart(chart.Data);
                                break;
                            case "3":
                                var topNums = await client.Dashboard.GetTopNumbersAsync(6);
                                ConsoleDisplayHelper.PrintTopNumbers(topNums.Data);
                                break;
                            default:
                                Console.WriteLine("Invalid dashboard option.");
                                break;
                        }
                        break;
                    case "5":
                        Console.WriteLine("\n--- Manage Messages (Encrypted) ---");
                        Console.WriteLine("1. List Messages");
                        Console.WriteLine("2. Get Message Details");
                        Console.WriteLine("3. Delete Messages");
                        Console.WriteLine("4. Send Message");
                        Console.Write("Choose an option: ");
                        var msgMenuChoice = Console.ReadLine();
                        switch (msgMenuChoice)
                        {
                            case "1":
                                Console.Write("Enter page number (default 1): ");
                                var pageInput = Console.ReadLine();
                                var pageNum = int.TryParse(pageInput, out int p) ? p : 1;
                                var messages = await client.Messages.ListAsync(new PaginationRequest { Page = pageNum });
                                if (messages.Data != null)
                                    ConsoleDisplayHelper.PrintMessages(messages.Data);
                                else
                                    Console.WriteLine("No messages returned.");
                                break;
                            case "2":
                                Console.Write("Enter Message ID: ");
                                if (int.TryParse(Console.ReadLine(), out int msgId))
                                {
                                    var msgDetail = await client.Messages.GetAsync(msgId);
                                    if (msgDetail.Data != null)
                                        ConsoleDisplayHelper.PrintMessageDetail(msgDetail.Data);
                                    else
                                        Console.WriteLine($"No message found with ID {msgId}.");
                                }
                                else { Console.WriteLine("Invalid Message ID."); }
                                break;
                            case "3":
                                Console.Write("Enter Message ID(s) to delete (comma-separated): ");
                                var idsInput = Console.ReadLine();
                                var deleteIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (idsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int did))
                                        deleteIds.Add(did);
                                }
                                if (deleteIds.Count == 0) { Console.WriteLine("No valid IDs provided."); break; }
                                var deleteResult = await client.Messages.DeleteAsync(new WhatsPro.Models.DeleteRequest { Ids = deleteIds });
                                Console.WriteLine($"API says: {deleteResult.Message}");
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
                            default:
                                Console.WriteLine("Invalid option.");
                                break;
                        }
                        break;
                    case "6":
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
                    case "7":
                        var token = await client.Auth.GetApiTokenAsync();
                        Console.WriteLine($"Your API Token is: {token}");
                        break;
                    case "8":
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
                    case "9":
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

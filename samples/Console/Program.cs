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

        ConsoleDisplayHelper.PrintInfo("WhatsPro.Net Interactive Sample");
        ConsoleDisplayHelper.PrintPrompt("Enter BaseUrl (e.g. https://whats-pro.net/backend/public/index.php/api): ");
        var baseUrl = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(baseUrl))
            baseUrl = "https://whats-pro.net/backend/public/index.php/api";

        // Email — use secret as default if available
        string email;
        if (!string.IsNullOrWhiteSpace(secretEmail))
        {
            ConsoleDisplayHelper.PrintPrompt($"Enter Email (press Enter to use '{secretEmail}'): ");
            var input = Console.ReadLine();
            email = string.IsNullOrWhiteSpace(input) ? secretEmail! : input!;
        }
        else
        {
            ConsoleDisplayHelper.PrintPrompt("Enter Email: ");
            email = Console.ReadLine()!;
        }

        // Password — use secret as default if available
        string password;
        if (!string.IsNullOrWhiteSpace(secretPassword))
        {
            ConsoleDisplayHelper.PrintPrompt("Enter Password (press Enter to use saved secret): ");
            var input = Console.ReadLine();
            password = string.IsNullOrWhiteSpace(input) ? secretPassword! : input!;
        }
        else
        {
            ConsoleDisplayHelper.PrintPrompt("Enter Password: ");
            password = Console.ReadLine()!;
        }

        ConsoleDisplayHelper.PrintInfo("─────────────────────────────────────────────");

        var options = new WhatsProOptions
        {
            BaseUrl = baseUrl!,
            Email = email!,
            Password = password!
        };

        using var client = new WhatsProClient(options);

        while (true)
        {
            ConsoleDisplayHelper.PrintWarning("\n--- Menu ---");
            Console.WriteLine("1. Get Profile (Login implicitly)");
            ConsoleDisplayHelper.PrintInfo("2. Groups Menu");
            ConsoleDisplayHelper.PrintInfo("3. Clients Menu");
            ConsoleDisplayHelper.PrintInfo("4. Dashboard Menu");
            Console.WriteLine("5. Manage Messages (Encrypted)");
            Console.WriteLine("6. Send Message (Non-Encrypted)");
            ConsoleDisplayHelper.PrintInfo("7. View API Token");
            ConsoleDisplayHelper.PrintInfo("8. Sessions Menu");
            ConsoleDisplayHelper.PrintInfo("9. Exit");
            ConsoleDisplayHelper.PrintPrompt("Choose an option: ");
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
                            ConsoleDisplayHelper.PrintWarning("Profile retrieved but user data was unavailable.");
                        break;
                    case "2":
                        ConsoleDisplayHelper.PrintWarning("\n--- Groups Menu ---");
                        ConsoleDisplayHelper.PrintInfo("1. List Groups");
                        ConsoleDisplayHelper.PrintInfo("2. Get Group by ID");
                        ConsoleDisplayHelper.PrintInfo("3. Get All Groups");
                        ConsoleDisplayHelper.PrintInfo("4. Create Group");
                        ConsoleDisplayHelper.PrintInfo("5. Update Group");
                        Console.WriteLine("6. Delete Group(s)");
                        ConsoleDisplayHelper.PrintInfo("7. Transfer Clients to Another Group");
                        ConsoleDisplayHelper.PrintInfo("8. Delete Group's Clients");
                        ConsoleDisplayHelper.PrintPrompt("Choose an option: ");
                        var groupChoice = Console.ReadLine();
                        switch (groupChoice)
                        {
                            case "1":
                                ConsoleDisplayHelper.PrintPrompt("Enter page number (default 1): ");
                                var pageInput = Console.ReadLine();
                                var pageNum = int.TryParse(pageInput, out int p) ? p : 1;
                                var groups = await client.Groups.ListAsync(new PaginationRequest { Page = pageNum });
                                if (groups.Data != null)
                                    ConsoleDisplayHelper.PrintGroups(groups.Data);
                                else
                                    ConsoleDisplayHelper.PrintWarning("No groups returned.");
                                break;
                            case "2":
                                ConsoleDisplayHelper.PrintPrompt("Enter Group ID: ");
                                if (int.TryParse(Console.ReadLine(), out int groupId))
                                {
                                    var groupInfo = await client.Groups.GetAsync(groupId);
                                    if (groupInfo.Data != null)
                                        ConsoleDisplayHelper.PrintGroupDetail(groupInfo.Data);
                                    else
                                        ConsoleDisplayHelper.PrintWarning($"No group found with ID {groupId}.");
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Group ID."); }
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
                                    ConsoleDisplayHelper.PrintWarning("No groups returned.");
                                }
                                break;
                            case "4":
                                ConsoleDisplayHelper.PrintPrompt("Enter Group Name: ");
                                var cName = Console.ReadLine() ?? "";
                                ConsoleDisplayHelper.PrintPrompt("Enter Notes (optional): ");
                                var cNotes = Console.ReadLine();
                                
                                var createReq = new WhatsPro.Groups.Models.CreateGroupRequest
                                {
                                    Name = cName,
                                    Notes = cNotes
                                };
                                var createRes = await client.Groups.CreateAsync(createReq);
                                ConsoleDisplayHelper.PrintApiResponse(createRes);
                                break;
                            case "5":
                                ConsoleDisplayHelper.PrintPrompt("Enter Group ID to Update: ");
                                if (int.TryParse(Console.ReadLine(), out int uId))
                                {
                                    ConsoleDisplayHelper.PrintPrompt("Enter Name: ");
                                    var uName = Console.ReadLine() ?? "";
                                    ConsoleDisplayHelper.PrintPrompt("Enter Notes (optional): ");
                                    var uNotes = Console.ReadLine();

                                    var updateReq = new WhatsPro.Groups.Models.UpdateGroupRequest
                                    {
                                        Name = uName,
                                        Notes = uNotes
                                    };
                                    var updateRes = await client.Groups.UpdateAsync(uId, updateReq);
                                    ConsoleDisplayHelper.PrintApiResponse(updateRes);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Group ID."); }
                                break;
                            case "6":
                                ConsoleDisplayHelper.PrintPrompt("Enter Group ID(s) to delete (comma-separated): ");
                                var dIdsInput = Console.ReadLine();
                                var deleteIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (dIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int did))
                                        deleteIds.Add(did);
                                }
                                if (deleteIds.Count == 0) { ConsoleDisplayHelper.PrintError("No valid IDs provided."); break; }
                                var deleteRes = await client.Groups.DeleteAsync(new WhatsPro.Models.DeleteRequest { Ids = deleteIds });
                                ConsoleDisplayHelper.PrintApiResponse(deleteRes);
                                break;
                            case "7":
                                ConsoleDisplayHelper.PrintPrompt("Enter Group ID(s) whose clients to transfer (comma-separated): ");
                                var tIdsInput = Console.ReadLine();
                                var transferIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (tIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int tid))
                                        transferIds.Add(tid);
                                }
                                if (transferIds.Count == 0) { ConsoleDisplayHelper.PrintError("No valid source Group IDs provided."); break; }
                                ConsoleDisplayHelper.PrintPrompt("Enter Target Group ID: ");
                                if (int.TryParse(Console.ReadLine(), out int tGroupId))
                                {
                                    var transferReq = new WhatsPro.Groups.Models.TransferClientsRequest
                                    {
                                        Ids = transferIds,
                                        GroupId = tGroupId
                                    };
                                    var transferRes = await client.Groups.TransferClientsAsync(transferReq);
                                    ConsoleDisplayHelper.PrintApiResponse(transferRes);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Target Group ID."); }
                                break;
                            case "8":
                                ConsoleDisplayHelper.PrintPrompt("Enter Group ID(s) to delete clients from (comma-separated): ");
                                var dcIdsInput = Console.ReadLine();
                                var deleteClientIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (dcIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int dcid))
                                        deleteClientIds.Add(dcid);
                                }
                                if (deleteClientIds.Count == 0) { ConsoleDisplayHelper.PrintError("No valid Group IDs provided."); break; }
                                var deleteClientsReq = new WhatsPro.Groups.Models.DeleteGroupClientsRequest { Ids = deleteClientIds };
                                var deleteClientsRes = await client.Groups.DeleteClientsAsync(deleteClientsReq);
                                ConsoleDisplayHelper.PrintApiResponse(deleteClientsRes);
                                break;
                            default:
                                ConsoleDisplayHelper.PrintError("Invalid group option.");
                                break;
                        }
                        break;

                    case "3":
                        ConsoleDisplayHelper.PrintWarning("\n--- Clients Menu ---");
                        ConsoleDisplayHelper.PrintInfo("1. List Clients");
                        ConsoleDisplayHelper.PrintInfo("2. Get Client by ID");
                        ConsoleDisplayHelper.PrintInfo("3. Create Client");
                        ConsoleDisplayHelper.PrintInfo("4. Update Client");
                        Console.WriteLine("5. Delete Client(s)");
                        ConsoleDisplayHelper.PrintInfo("6. Add Extra Phone");
                        ConsoleDisplayHelper.PrintInfo("7. Change Group");
                        ConsoleDisplayHelper.PrintInfo("8. Import From Excel");
                        ConsoleDisplayHelper.PrintPrompt("Choose an option: ");
                        var clientChoice = Console.ReadLine();
                        switch (clientChoice)
                        {
                            case "1":
                                ConsoleDisplayHelper.PrintPrompt("Enter page number (default 1): ");
                                var pageInput = Console.ReadLine();
                                var pageNum = int.TryParse(pageInput, out int p) ? p : 1;
                                var clients = await client.Clients.ListAsync(new PaginationRequest { Page = pageNum });
                                if (clients.Data != null)
                                    ConsoleDisplayHelper.PrintClients(clients.Data);
                                else
                                    ConsoleDisplayHelper.PrintWarning("No clients returned.");
                                break;
                            case "2":
                                ConsoleDisplayHelper.PrintPrompt("Enter Client ID: ");
                                if (int.TryParse(Console.ReadLine(), out int clientId))
                                {
                                    var clientInfo = await client.Clients.GetAsync(clientId);
                                    if (clientInfo.Data != null)
                                        ConsoleDisplayHelper.PrintClientDetail(clientInfo.Data);
                                    else
                                        ConsoleDisplayHelper.PrintWarning($"No client found with ID {clientId}.");
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Client ID."); }
                                break;
                            case "3":
                                ConsoleDisplayHelper.PrintPrompt("Enter Phone (e.g. +2010...): ");
                                var cPhone = Console.ReadLine() ?? "";
                                ConsoleDisplayHelper.PrintPrompt("Enter Name: ");
                                var cName = Console.ReadLine() ?? "";
                                var cGroupsRes = await client.Groups.GetAllAsync();
                                if (cGroupsRes.Data != null && cGroupsRes.Data.Count > 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("  Available Groups: " + string.Join(", ", cGroupsRes.Data.ConvertAll(g => $"[{g.Id}] {g.Name}")));
                                    Console.ResetColor();
                                }
                                ConsoleDisplayHelper.PrintPrompt("Enter Group ID: ");
                                int cGroupId = int.TryParse(Console.ReadLine(), out int cgid) ? cgid : 0;
                                ConsoleDisplayHelper.PrintPrompt("Enter Notes (optional): ");
                                var cNotes = Console.ReadLine();
                                
                                var createReq = new WhatsPro.Clients.Models.CreateClientRequest
                                {
                                    Phone = cPhone,
                                    Name = cName,
                                    GroupId = cGroupId,
                                    Notes = cNotes
                                };
                                var createRes = await client.Clients.CreateAsync(createReq);
                                ConsoleDisplayHelper.PrintApiResponse(createRes);
                                break;
                            case "4":
                                ConsoleDisplayHelper.PrintPrompt("Enter Client ID to Update: ");
                                if (int.TryParse(Console.ReadLine(), out int uId))
                                {
                                    ConsoleDisplayHelper.PrintPrompt("Enter Name: ");
                                    var uName = Console.ReadLine() ?? "";
                                    ConsoleDisplayHelper.PrintPrompt("Enter Phone: ");
                                    var uPhone = Console.ReadLine() ?? "";
                                    var uGroupsRes = await client.Groups.GetAllAsync();
                                    if (uGroupsRes.Data != null && uGroupsRes.Data.Count > 0)
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                        Console.WriteLine("  Available Groups: " + string.Join(", ", uGroupsRes.Data.ConvertAll(g => $"[{g.Id}] {g.Name}")));
                                        Console.ResetColor();
                                    }
                                    ConsoleDisplayHelper.PrintPrompt("Enter Group ID: ");
                                    int uGroupId = int.TryParse(Console.ReadLine(), out int ugid) ? ugid : 0;
                                    ConsoleDisplayHelper.PrintPrompt("Enter Notes (optional): ");
                                    var uNotes = Console.ReadLine();

                                    var updateReq = new WhatsPro.Clients.Models.UpdateClientRequest
                                    {
                                        Name = uName,
                                        Phone = uPhone,
                                        GroupId = uGroupId,
                                        Notes = uNotes
                                    };
                                    var updateRes = await client.Clients.UpdateAsync(uId, updateReq);
                                    ConsoleDisplayHelper.PrintApiResponse(updateRes);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Client ID."); }
                                break;
                            case "5":
                                ConsoleDisplayHelper.PrintPrompt("Enter Client ID(s) to delete (comma-separated): ");
                                var dIdsInput = Console.ReadLine();
                                var deleteClientIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (dIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int did))
                                        deleteClientIds.Add(did);
                                }
                                if (deleteClientIds.Count == 0) { ConsoleDisplayHelper.PrintError("No valid IDs provided."); break; }
                                var deleteRes = await client.Clients.DeleteAsync(new WhatsPro.Models.DeleteRequest { Ids = deleteClientIds });
                                ConsoleDisplayHelper.PrintApiResponse(deleteRes);
                                break;
                            case "6":
                                ConsoleDisplayHelper.PrintPrompt("Enter Client ID to add phone to: ");
                                if (int.TryParse(Console.ReadLine(), out int pClientId))
                                {
                                    ConsoleDisplayHelper.PrintPrompt("Enter Extra Phone Number: ");
                                    var pPhone = Console.ReadLine() ?? "";
                                    var addPhoneReq = new WhatsPro.Clients.Models.AddPhoneRequest
                                    {
                                        ClientId = pClientId,
                                        Phone = pPhone
                                    };
                                    var addPhoneRes = await client.Clients.AddPhoneAsync(addPhoneReq);
                                    ConsoleDisplayHelper.PrintApiResponse(addPhoneRes);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Client ID."); }
                                break;
                            case "7":
                                ConsoleDisplayHelper.PrintPrompt("Enter Client ID(s) to change group (comma-separated): ");
                                var cIdsInput = Console.ReadLine();
                                var changeIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (cIdsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int cid))
                                        changeIds.Add(cid);
                                }
                                if (changeIds.Count == 0) { ConsoleDisplayHelper.PrintError("No valid IDs provided."); break; }
                                var tGroupsRes = await client.Groups.GetAllAsync();
                                if (tGroupsRes.Data != null && tGroupsRes.Data.Count > 0)
                                {
                                    Console.ForegroundColor = ConsoleColor.DarkGray;
                                    Console.WriteLine("  Available Groups: " + string.Join(", ", tGroupsRes.Data.ConvertAll(g => $"[{g.Id}] {g.Name}")));
                                    Console.ResetColor();
                                }
                                ConsoleDisplayHelper.PrintPrompt("Enter Target Group ID: ");
                                if (int.TryParse(Console.ReadLine(), out int tGroupId))
                                {
                                    var changeGroupReq = new WhatsPro.Clients.Models.ChangeGroupRequest
                                    {
                                        Ids = changeIds,
                                        GroupId = tGroupId
                                    };
                                    var changeRes = await client.Clients.ChangeGroupAsync(changeGroupReq);
                                    ConsoleDisplayHelper.PrintApiResponse(changeRes);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Target Group ID."); }
                                break;
                            case "8":
                                ConsoleDisplayHelper.PrintPrompt("Enter path to Excel file: ");
                                var excelPath = Console.ReadLine()?.Trim('"');
                                if (string.IsNullOrWhiteSpace(excelPath) || !System.IO.File.Exists(excelPath))
                                {
                                    ConsoleDisplayHelper.PrintError($"File not found: {excelPath}");
                                    break;
                                }
                                ConsoleDisplayHelper.PrintInfo("Uploading Excel file...");
                                using (var stream = System.IO.File.OpenRead(excelPath))
                                {
                                    var importRes = await client.Clients.ImportFromExcelAsync(stream);
                                    ConsoleDisplayHelper.PrintApiResponse(importRes);
                                }
                                break;
                            default:
                                ConsoleDisplayHelper.PrintError("Invalid client option.");
                                break;
                        }
                        break;
                    case "4":
                        ConsoleDisplayHelper.PrintWarning("\n--- Dashboard Menu ---");
                        ConsoleDisplayHelper.PrintInfo("1. Dashboard Summary");
                        ConsoleDisplayHelper.PrintInfo("2. Send Chart");
                        ConsoleDisplayHelper.PrintInfo("3. Top Numbers");
                        ConsoleDisplayHelper.PrintPrompt("Choose an option: ");
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
                                ConsoleDisplayHelper.PrintError("Invalid dashboard option.");
                                break;
                        }
                        break;
                    case "5":
                        ConsoleDisplayHelper.PrintWarning("\n--- Manage Messages (Encrypted) ---");
                        ConsoleDisplayHelper.PrintInfo("1. List Messages");
                        ConsoleDisplayHelper.PrintInfo("2. Get Message Details");
                        ConsoleDisplayHelper.PrintInfo("3. Delete Messages");
                        ConsoleDisplayHelper.PrintInfo("4. Send Message");
                        ConsoleDisplayHelper.PrintPrompt("Choose an option: ");
                        var msgMenuChoice = Console.ReadLine();
                        switch (msgMenuChoice)
                        {
                            case "1":
                                ConsoleDisplayHelper.PrintPrompt("Enter page number (default 1): ");
                                var pageInput = Console.ReadLine();
                                var pageNum = int.TryParse(pageInput, out int p) ? p : 1;
                                var messages = await client.Messages.ListAsync(new PaginationRequest { Page = pageNum });
                                if (messages.Data != null)
                                    ConsoleDisplayHelper.PrintMessages(messages.Data);
                                else
                                    ConsoleDisplayHelper.PrintInfo("No messages returned.");
                                break;
                            case "2":
                                ConsoleDisplayHelper.PrintPrompt("Enter Message ID: ");
                                if (int.TryParse(Console.ReadLine(), out int msgId))
                                {
                                    var msgDetail = await client.Messages.GetAsync(msgId);
                                    if (msgDetail.Data != null)
                                        ConsoleDisplayHelper.PrintMessageDetail(msgDetail.Data);
                                    else
                                        ConsoleDisplayHelper.PrintInfo($"No message found with ID {msgId}.");
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Message ID."); }
                                break;
                            case "3":
                                ConsoleDisplayHelper.PrintPrompt("Enter Message ID(s) to delete (comma-separated): ");
                                var idsInput = Console.ReadLine();
                                var deleteIds = new System.Collections.Generic.List<int>();
                                foreach (var part in (idsInput ?? "").Split(','))
                                {
                                    if (int.TryParse(part.Trim(), out int did))
                                        deleteIds.Add(did);
                                }
                                if (deleteIds.Count == 0) { ConsoleDisplayHelper.PrintError("No valid IDs provided."); break; }
                                var deleteResult = await client.Messages.DeleteAsync(new WhatsPro.Models.DeleteRequest { Ids = deleteIds });
                                ConsoleDisplayHelper.PrintApiResponse(deleteResult);
                                break;
                            case "4":
                                ConsoleDisplayHelper.PrintPrompt("Enter target phone number (e.g. 2010...): ");
                                var targetPhone = Console.ReadLine();
                                ConsoleDisplayHelper.PrintPrompt("Enter message text: ");
                                var msgText = Console.ReadLine();
                                var msgRequest = new Messages.Models.SendMessageRequest
                                {
                                    SendPhone = true,
                                    Phones = new System.Collections.Generic.List<string> { targetPhone! },
                                    Message = msgText!
                                };
                                var sendResult = await client.Messages.SendAsync(msgRequest);
                                ConsoleDisplayHelper.PrintApiResponse(sendResult, "Success: Message sent. API says:");
                                break;
                            default:
                                ConsoleDisplayHelper.PrintError("Invalid option.");
                                break;
                        }
                        break;
                    case "6":
                        ConsoleDisplayHelper.PrintWarning("\n--- Send Non-Encrypted Message ---");
                        ConsoleDisplayHelper.PrintInfo("1. Full phone format");
                        ConsoleDisplayHelper.PrintInfo("2. Country code");
                        ConsoleDisplayHelper.PrintInfo("3. Image URL");
                        ConsoleDisplayHelper.PrintInfo("4. Base64 Image");
                        ConsoleDisplayHelper.PrintInfo("5. Document");
                        ConsoleDisplayHelper.PrintPrompt("Choose type: ");
                        var typeChoice = Console.ReadLine();
                        
                        var msgRequestNon = new Messages.Models.SendMessageRequest { SendPhone = true };

                        if (typeChoice == "1" || typeChoice == "2" || typeChoice == "3" || typeChoice == "4" || typeChoice == "5")
                        {
                            ConsoleDisplayHelper.PrintPrompt("Enter target phone number: ");
                            var targetPhoneNon = Console.ReadLine();
                            msgRequestNon.Phones = new System.Collections.Generic.List<string> { targetPhoneNon! };
                            
                            ConsoleDisplayHelper.PrintPrompt("Enter message text: ");
                            msgRequestNon.Message = Console.ReadLine()!;
                            
                            if (typeChoice == "2" || typeChoice == "3" || typeChoice == "4")
                            {
                                ConsoleDisplayHelper.PrintPrompt("Enter country code (e.g. EG): ");
                                msgRequestNon.CountryCode = Console.ReadLine()!;
                            }
                            
                            if (typeChoice == "3")
                            {
                                ConsoleDisplayHelper.PrintPrompt("Enter Image URL: ");
                                msgRequestNon.ImgUrl = Console.ReadLine()!;
                            }
                            else if (typeChoice == "4")
                            {
                                ConsoleDisplayHelper.PrintPrompt("Enter Base64 Image data: ");
                                msgRequestNon.Img = Console.ReadLine()!;
                            }
                            else if (typeChoice == "5")
                            {
                                ConsoleDisplayHelper.PrintPrompt("Enter file path to upload: ");
                                var filePath = Console.ReadLine()?.Trim('"');
                                if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
                                {
                                    ConsoleDisplayHelper.PrintError($"File not found: {filePath}");
                                    break;
                                }
                                ConsoleDisplayHelper.PrintInfo("Uploading document...");
                                using var stream = System.IO.File.OpenRead(filePath);
                                var fileName = System.IO.Path.GetFileName(filePath);
                                var uploadResult = await client.Messages.UploadDocumentAsync(fileName, stream);
                                if (uploadResult.Success && uploadResult.Data != null)
                                {
                                    msgRequestNon.DocId = uploadResult.Data.Id;
                                    ConsoleDisplayHelper.PrintInfo($"Document uploaded. ID: {msgRequestNon.DocId}");
                                }
                                else
                                {
                                    ConsoleDisplayHelper.PrintInfo($"Upload failed: {uploadResult.Message}");
                                    break;
                                }
                            }
                            
                            var sendResultNon = await client.Messages.SendNonEncryptedAsync(msgRequestNon);
                            ConsoleDisplayHelper.PrintApiResponse(sendResultNon, "Success: Message sent (Non-Encrypted). API says:");
                        }
                        else
                        {
                            ConsoleDisplayHelper.PrintError("Invalid type choice.");
                        }
                        break;
                    case "7":
                        var token = await client.Auth.GetApiTokenAsync();
                        ConsoleDisplayHelper.PrintSuccess($"Your API Token is: {token}");
                        break;
                    case "8":
                        ConsoleDisplayHelper.PrintWarning("\n--- Sessions Menu ---");
                        ConsoleDisplayHelper.PrintInfo("1. List Sessions");
                        ConsoleDisplayHelper.PrintInfo("2. Get Session by ID");
                        ConsoleDisplayHelper.PrintInfo("3. Connect Session");
                        ConsoleDisplayHelper.PrintInfo("4. Disconnect Session");
                        ConsoleDisplayHelper.PrintInfo("5. Change Session Name");
                        ConsoleDisplayHelper.PrintInfo("6. Set Webhook URL");
                        ConsoleDisplayHelper.PrintPrompt("Choose an option: ");
                        var sessionChoice = Console.ReadLine();
                        switch (sessionChoice)
                        {
                            case "1":
                                var sessions = await client.Sessions.ListAsync(new PaginationRequest());
                                ConsoleDisplayHelper.PrintSessions(sessions.Data);
                                break;
                            case "2":
                                ConsoleDisplayHelper.PrintPrompt("Enter Session ID: ");
                                if (int.TryParse(Console.ReadLine(), out int sessionId))
                                {
                                    var session = await client.Sessions.GetAsync(sessionId);
                                    if (session != null && session.Data != null)
                                        ConsoleDisplayHelper.PrintInfo($"Session: {session.Data.Name} - Status: {session.Data.Status}");
                                    else
                                        ConsoleDisplayHelper.PrintError($"Failed to get session: {session?.Message ?? "No response"}");
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Session ID."); }
                                break;
                            case "3":
                                ConsoleDisplayHelper.PrintPrompt("Enter Session ID to Connect: ");
                                if (int.TryParse(Console.ReadLine(), out int cId))
                                {
                                    var result = await client.Sessions.ConnectAsync(cId);
                                    if (result != null)
                                    {
                                        ConsoleDisplayHelper.PrintApiResponse(result);
                                        if (result.Data != null && !string.IsNullOrEmpty(result.Data.Qr))
                                        {
                                            if (result.Data.Qr == "connected" || result.Data.Qr == "qr")
                                            {
                                                ConsoleDisplayHelper.PrintInfo($"Status: {result.Data.Qr}");
                                            }
                                            else
                                            {
                                                ConsoleDisplayHelper.PrintInfo($"QR Code received. Attempting to open...");
                                                try
                                                {
                                                    string base64 = result.Data.Qr.Replace("data:image/png;base64,", "");
                                                    byte[] imageBytes = Convert.FromBase64String(base64);
                                                    string filePath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"qr_{cId}.png");
                                                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                                                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(filePath) { UseShellExecute = true });
                                                    ConsoleDisplayHelper.PrintInfo($"Opened QR Code from {filePath}");
                                                }
                                                catch (Exception ex)
                                                {
                                                    ConsoleDisplayHelper.PrintInfo($"Failed to open QR Code: {ex.Message}");
                                                    ConsoleDisplayHelper.PrintInfo($"QR Code data length: {result.Data.Qr.Length}");
                                                }
                                            }
                                        }
                                    }
                                    else { ConsoleDisplayHelper.PrintInfo("API returned null response."); }
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Session ID."); }
                                break;
                            case "4":
                                ConsoleDisplayHelper.PrintPrompt("Enter Session ID to Disconnect: ");
                                if (int.TryParse(Console.ReadLine(), out int dId))
                                {
                                    ConsoleDisplayHelper.PrintPrompt("Disconnect forever? (y/n): ");
                                    bool forever = Console.ReadLine()?.Trim().ToLower() == "y";
                                    var result = await client.Sessions.DisconnectAsync(dId, forever);
                                    ConsoleDisplayHelper.PrintApiResponse(result);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Session ID."); }
                                break;
                            case "5":
                                ConsoleDisplayHelper.PrintPrompt("Enter Session ID to Change Name: ");
                                if (int.TryParse(Console.ReadLine(), out int cnId))
                                {
                                    var result = await client.Sessions.ChangeNameAsync(cnId);
                                    ConsoleDisplayHelper.PrintApiResponse(result);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Session ID."); }
                                break;
                            case "6":
                                ConsoleDisplayHelper.PrintPrompt("Enter Session ID to Set Webhook: ");
                                if (int.TryParse(Console.ReadLine(), out int wId))
                                {
                                    ConsoleDisplayHelper.PrintPrompt("Enter new Webhook URL: ");
                                    var wUrl = Console.ReadLine();
                                    var result = await client.Sessions.SetWebhookAsync(wId, new WhatsPro.Sessions.Models.SetWebhookRequest { Url = wUrl ?? string.Empty });
                                    ConsoleDisplayHelper.PrintApiResponse(result);
                                }
                                else { ConsoleDisplayHelper.PrintError("Invalid Session ID."); }
                                break;
                            default:
                                ConsoleDisplayHelper.PrintError("Invalid session option.");
                                break;
                        }
                        break;
                    case "9":
                        return;
                    default:
                        ConsoleDisplayHelper.PrintError("Invalid option.");
                        break;
                }
            }
            catch (Exception ex)
            {
                ConsoleDisplayHelper.PrintError($"Error: {ex.GetType().Name} - {ex.Message}");
            }
        }
    }
}

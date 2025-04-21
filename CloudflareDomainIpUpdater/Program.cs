using CloudFlare.Client;
using CloudFlare.Client.Api.Authentication;
using CloudFlare.Client.Api.Result;
using CloudFlare.Client.Api.Zones.DnsRecord;

namespace CloudflareDomainIpUpdater;

internal class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 4)
        {
            string exeName = Path.GetFileName(Environment.ProcessPath!);
            Console.WriteLine("Kullanım:");
            Print(exeName, ConsoleColor.Cyan);
            Print("--global", ConsoleColor.Yellow);
            Print("<E-Posta>", ConsoleColor.Yellow);
            Print("<Global-API-Key>", ConsoleColor.Yellow);
            Print("<Eski.IP>", ConsoleColor.Red);
            PrintLine("<Yeni.IP>", ConsoleColor.Green);
            Console.WriteLine("veya");
            Print(exeName, ConsoleColor.Cyan);
            Print("--token", ConsoleColor.Yellow);
            Print("<Access-Token>", ConsoleColor.Yellow);
            Print("<Eski.IP>", ConsoleColor.Red);
            PrintLine("<Yeni.IP>", ConsoleColor.Green);

            Console.WriteLine();
            Console.WriteLine("Çıkmak için herhangi bir tuşa basın.");
            Console.ReadKey();
            return;
        }
        try
        {
            string authType = args[0];
            string oldContent;
            string newContent;
            IAuthentication authentication;
            if (authType.Equals("--global", StringComparison.OrdinalIgnoreCase))
            {
                authentication = new ApiKeyAuthentication(args[1], args[2]);
                oldContent = args[3];
                newContent = args[4];
            }
            else if (authType.Equals("--token", StringComparison.OrdinalIgnoreCase))
            {
                authentication = new ApiTokenAuthentication(args[1]);
                oldContent = args[2];
                newContent = args[3];
            }
            else
            {
                Console.WriteLine("Geçersiz kimlik doğrulama türü. Lütfen '--global' veya '--token' kullanın.");
                return;
            }
            using var client = new CloudFlareClient(authentication);
            int page = 1;
            int perPage = 20;
            int totalPages = 1;

            do
            {
                var zonesResponse = await client.Zones.GetAsync(displayOptions: new CloudFlare.Client.Api.Display.DisplayOptions
                {
                    Page = page,
                    PerPage = perPage
                });

                if (!zonesResponse.Success)
                {
                    PrintError(zonesResponse);
                    return;
                }

                totalPages = zonesResponse.ResultInfo.TotalPage;

                if (zonesResponse.Result == null || zonesResponse.Result.Count == 0)
                    break;

                foreach (var zone in zonesResponse.Result)
                {
                    var dnsRecordsResponse = await client.Zones.DnsRecords.GetAsync(zone.Id);
                    if (!dnsRecordsResponse.Success)
                    {
                        PrintError(dnsRecordsResponse);
                        continue;
                    }
                    foreach (DnsRecord? record in dnsRecordsResponse.Result)
                    {
                        if (record.Content.Contains(oldContent, StringComparison.OrdinalIgnoreCase))
                        {
                            var replaceContent = record.Content.Replace(oldContent, newContent, StringComparison.OrdinalIgnoreCase);
                            var modified = new ModifiedDnsRecord
                            {
                                Type = record.Type,
                                Name = record.Name,
                                Content = replaceContent,
                                Proxied = record.Proxied,
                                Ttl = record.Ttl,
                                Priority = record.Priority
                            };
                            var updateResponse = await client.Zones.DnsRecords.UpdateAsync(zone.Id, record.Id, modified);
                            if (updateResponse.Success)
                            {
                                Print("Güncellendi:", ConsoleColor.Green);
                                Print(record.Name, ConsoleColor.Cyan);
                                Print(record.Type.ToString(), ConsoleColor.Magenta);
                                Print(record.Content, ConsoleColor.Yellow);
                                Print("=>", ConsoleColor.White);
                                PrintLine(replaceContent, ConsoleColor.Blue);
                            }
                            else
                                PrintError(updateResponse);
                        }
                    }
                }

                page++;
            }
            while (page <= totalPages);

            PrintLine("Tüm kayıtlar güncellendi.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            PrintLine(ex.ToString(), ConsoleColor.Red);
        }
    }

    private static void Print(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.Write($" {message} ");
        Console.ResetColor();
    }

    private static void PrintLine(string message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static void PrintError<T>(CloudFlareResult<T> result)
    {
        if (!result.Success)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            if (result.Errors != null && result.Errors.Count > 0)
                foreach (var error in result.Errors)
                    Console.WriteLine(error.Message);

            if (result.Messages != null && result.Messages.Count > 0)
                foreach (var message in result.Messages)
                    Console.WriteLine(message);
            Console.ResetColor();
        }
    }
}

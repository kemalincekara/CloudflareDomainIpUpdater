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
            Console.WriteLine($"{exeName} <E_Posta> <Global_API_Key> <Eski_IP> <Yeni_IP>");
            Console.WriteLine();
            Console.WriteLine("Çıkmak için herhangi bir tuşa basın.");
            Console.ReadKey();
            return;
        }
        string email = args[0];
        string key = args[1];
        string oldIp = args[2];
        string newIp = args[3];
        try
        {
            using var client = new CloudFlareClient(new ApiKeyAuthentication(email, key));

            var zones = await client.Zones.GetAsync();

            if (!zones.Success)
            {
                PrintError(zones);
                return;
            }

            foreach (var zone in zones.Result)
            {
                var dnsRecords = await client.Zones.DnsRecords.GetAsync(zone.Id);
                foreach (DnsRecord? record in dnsRecords.Result)
                {
                    if (record.Content == oldIp)
                    {
                        var modified = new ModifiedDnsRecord
                        {
                            Type = record.Type,
                            Name = record.Name,
                            Content = newIp,
                            Proxied = record.Proxied,
                            Ttl = record.Ttl,
                            Priority = record.Priority
                        };
                        var update = await client.Zones.DnsRecords.UpdateAsync(zone.Id, record.Id, modified);
                        if (update.Success)
                            Print($"Güncellendi: {record.Name}, {record.Type}, {oldIp} => {newIp}", ConsoleColor.Green);
                        else
                            PrintError(update);
                    }
                }
            }

            Print("Tüm kayıtlar güncellendi.", ConsoleColor.Green);
        }
        catch (Exception ex)
        {
            Print(ex.ToString(), ConsoleColor.Red);
        }
    }

    private static void Print(string message, ConsoleColor color)
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

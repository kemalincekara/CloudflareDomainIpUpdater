# Cloudflare Domain IP Updater

*Türkçe versiyon için, lütfen [README.tr.md](README.tr.md)'e bakın.*

This application is a simple command line tool that updates the DNS records in your Cloudflare account using the Cloudflare API. All DNS records that match the specified "Old_IP" value are updated with "New_IP".

## Features

- **Automatic DNS Update:** Scans the DNS records in all zones in your Cloudflare account.
- **IP Change:** If the content of a DNS record matches the specified "Old_IP", the record is updated with "New_IP".
- **Error Management:** Errors that may occur during API calls are reported clearly on the console.
- **User Friendly:** If insufficient parameters are provided, usage information is displayed to inform the user.

## Requirements

- [.NET 9 or a newer version of .NET](https://dotnet.microsoft.com/download)
- [CloudFlare.Client](https://www.nuget.org/packages/CloudFlare.Client/) NuGet package
- A valid Cloudflare account and API access credentials (Email address and Global API Key)

## Installation

1. Clone or download the project:
   ```bash
   git clone https://github.com/kemalincekara/CloudflareDomainIpUpdater.git
   ```
2. Navigate to the project directory:
   ```bash
   cd CloudflareDomainIpUpdater
   ```
3. Install the required NuGet packages:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build
   ```

## Usage

To run the application, you must enter the following parameters in the command line:

```bash
CloudflareDomainIpUpdater.exe <Email> <Global_API_Key> <Old_IP> <New_IP>
```

**Parameter Descriptions:**

- `<Email>`: The email address associated with your Cloudflare account.
- `<Global_API_Key>`: Your Cloudflare Global API Key.
- `<Old_IP>`: The old IP address to be searched for in the DNS records to be updated.
- `<New_IP>`: The new IP address with which the matching DNS records will be updated.

### Example

In the following example, the value `192.168.1.67` will be updated to `10.0.0.67`:

```bash
CloudflareDomainIpUpdater.exe user@example.com abcdef1234567890oldapikey 192.168.1.67 10.0.0.67
```

## Code Explanation

- **Input Control:** At the start of the program, the number of command line arguments is checked. If insufficient parameters are provided, usage information is displayed.
- **API Connection:** The `CloudFlareClient` object is created using the email and API key.
- **Zone and DNS Record Operations:** 
  - All zones in the Cloudflare account are retrieved.
  - For each zone, all DNS records are queried.
  - If a record's content matches the specified old IP, it is updated with the new IP.
- **Error Management:** If an error occurs during API calls, detailed error messages are printed to the console.

## Contributing

All contributions, bug reports, or development suggestions are welcome. Please open an issue to discuss the matter before opening a pull request.

## License

This project is licensed under the MIT License. For more information, please refer to the `LICENSE` file.
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

        // Load configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        var configuration = builder.Build();

        var azureAdSettings = configuration.GetSection("AzureAd");
        var weatherApiSettings = configuration.GetSection("WeatherApi");

        string tenantId = azureAdSettings["TenantId"];
        string clientId = azureAdSettings["ClientId"];
        string callbackUri = azureAdSettings["CallbackUri"]; // Redirect URI for interactive flow
        string authority = $"{azureAdSettings["Instance"]}{tenantId}";

        string[] scopes = weatherApiSettings.GetSection("Scopes").Get<string[]>();
        string weatherApiBaseUrl = weatherApiSettings["BaseUrl"];

        var app = PublicClientApplicationBuilder.Create(clientId)
            .WithAuthority(authority)
            .WithRedirectUri(callbackUri)
            .Build();
        Console.WriteLine("Acquiring token...");

        AuthenticationResult result;
        try
        {
            // Try to acquire token silently (e.g., if cached from previous run)
            var accounts = await app.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();
            if (firstAccount != null)
            {
                result = await app.AcquireTokenSilent(scopes, firstAccount).ExecuteAsync();
                Console.WriteLine("Token acquired silently.");
            }
            else
            {
                throw new MsalUiRequiredException("no_account", "No account found in token cache.");
            }
        }
        catch (MsalUiRequiredException exception)
        {
            // If silent acquisition fails, interactive login is required
            Console.WriteLine("No token in cache, acquiring interactively...");
            try
            {
                result = await app.AcquireTokenInteractive(scopes).ExecuteAsync();
                Console.WriteLine("Token acquired interactively.");
            }
            catch (MsalException msalex)
            {
                Console.WriteLine($"Error acquiring token interactively: {msalex.Message}");
                return;
            }
        }
        catch (MsalException msalex)
        {
            Console.WriteLine($"Error acquiring token: {msalex.Message}");
            return;
        }
        Console.WriteLine($"Access Token: {result.AccessToken}");

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

        // Call the protected API requiring 'forecast.read' scope
        var protectedReadUrl = $"{weatherApiBaseUrl}/api/location/locations";
        Console.WriteLine($"\nCalling protected read API: {protectedReadUrl}");
        try
        {
            var response = await httpClient.GetAsync(protectedReadUrl);
            Console.WriteLine($"Protected read API Response Status: {response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response Content: {content}");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error Content: {errorContent}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling protected read API: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WeatherForecast.Domain.DataAccess;
using WeatherForecast.Domain.Entities;
using OpenAI;
using System.Text.Json.Serialization;
using OpenAI.Chat;
using Azure.AI.OpenAI;
using Azure; // Add this line if OpenAIService is in this namespace, otherwise use the correct one

namespace WeatherForecast.Domain.Business.Classes
{
    public class AzureOpenAiService : IAzureOpenAIService
    {
        private readonly AzureOpenAIClient _openAIClient;
        private readonly string _chatDeploymentName;
        private readonly WeatherForecastDbContext _dbContext; // Inject DbContext here

        // Constructor now takes both OpenAI client and DbContext
        public AzureOpenAiService(AzureOpenAIClient openAIClient, string chatDeploymentName, WeatherForecastDbContext dbContext)
        {
            _openAIClient = openAIClient;
            _chatDeploymentName = chatDeploymentName;
            _dbContext = dbContext; // Store DbContext instance
        }

        // --- Model for AI-generated query parameters (remains the same) ---
        public class LocationQueryParams
        {
            [JsonPropertyName("minLatitude")]
            public double? MinLatitude { get; set; }

            [JsonPropertyName("maxLatitude")]
            public double? MaxLatitude { get; set; }

            [JsonPropertyName("minLongitude")]
            public double? MinLongitude { get; set; }

            [JsonPropertyName("maxLongitude")]
            public double? MaxLongitude { get; set; }

            [JsonPropertyName("lastAccessedAfter")]
            public DateTime? LastAccessedAfter { get; set; }

            [JsonPropertyName("lastAccessedBefore")]
            public DateTime? LastAccessedBefore { get; set; }

            [JsonPropertyName("limit")]
            public int? Limit { get; set; }

            [JsonPropertyName("orderBy")]
            public string OrderBy { get; set; }

            [JsonPropertyName("summarize")]
            public bool Summarize { get; set; } = false;
        }

        // --- AI interaction: Extract Query Parameters ---
        private async Task<LocationQueryParams?> ExtractQueryParamsAsync(string naturalLanguageQuery)
        {
            // Get a ChatClient for the specific deployment
            ChatClient chatClient = _openAIClient.GetChatClient(_chatDeploymentName);

            // Messages are now created directly as ChatMessage instances or derived types
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    "You are a helpful assistant that extracts query parameters for a Locations database table. " +
                    "The table has columns: 'Id' (int), 'Latitude' (double), 'Longitude' (double), 'AccessedDateTime' (datetime). " +
                    "Respond ONLY with a JSON object containing the extracted parameters. " +
                    "If a parameter is not specified, omit it or set it to null. " +
                    "For date/time range queries, map to 'lastAccessedAfter' and 'lastAccessedBefore' in the JSON. " +
                    "For ordering, map to 'orderBy' in the JSON, with values like 'AccessedDateTime_desc', 'AccessedDateTime_asc', 'latitude_asc', etc. " +
                    "Example JSON: {\"minLatitude\": 34.0, \"maxLatitude\": 35.0, \"lastAccessedAfter\": \"2024-01-01T00:00:00Z\", \"limit\": 5, \"orderBy\": \"AccessedDateTime_desc\"}. " +
                    "Use ISO 8601 format for dates (e.g., 'YYYY-MM-DDTHH:MM:SSZ')."
                ),
                new UserChatMessage(naturalLanguageQuery)
            };

            var chatCompletionOptions = new ChatCompletionOptions()
            {                
                Temperature = 0.0f, 
                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
            };

            System.ClientModel.ClientResult<ChatCompletion> response = await chatClient.CompleteChatAsync(messages, chatCompletionOptions);

            string jsonResponse = response.Value.Content[0].Text;

            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<LocationQueryParams>(jsonResponse, options);
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"Error deserializing AI response to JSON: {ex.Message} - Response: {jsonResponse}");
                return null;
            }
        }

        // --- AI interaction: Summarize Results ---
        private async Task<string> SummarizeLocationsAsync(List<Location> locations, string originalQuery)
        {
            if (locations == null || !locations.Any())
            {
                return "No locations found matching your query.";
            }

            ChatClient chatClient = _openAIClient.GetChatClient(_chatDeploymentName); // Get ChatClient

            var locationStrings = locations.Select(loc =>
                $"ID: {loc.Id}, Lat: {loc.Latitude}, Lon: {loc.Longitude}, Accessed: {loc.AccessedDateTime:yyyy-MM-dd HH:mm}"
            ).ToList();

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    "You are a helpful assistant that summarizes location data. " +
                    "Given a list of locations and the original user query, provide a natural language summary of the results. " +
                    "Focus on key insights like the number of locations, their general area if applicable, and accessed times. " +
                    "Do not just list them. If there are many, mention the count."
                ),
                new UserChatMessage(
                    $"Original Query: '{originalQuery}'\n\n" +
                    $"Locations found:\n{string.Join("\n", locationStrings)}\n\n" +
                    "Provide a natural language summary."
                )
            };

            var chatCompletionsOptions = new ChatCompletionOptions()
            {
                Temperature = 0.5f
            };

            System.ClientModel.ClientResult<ChatCompletion> response = await chatClient.CompleteChatAsync(messages, chatCompletionsOptions);

            // This line remains correct for accessing the content from the ClientResult<T>
            string jsonResponse = response.Value.Content[0].Text;
            return jsonResponse;
        }

        // --- Combined Public Method for Natural Language Querying (remains the same logic flow) ---
        public async Task<LocationQueryResponse> AskLocationsAsync(string naturalLanguageQuery)
        {
            if (string.IsNullOrWhiteSpace(naturalLanguageQuery))
            {
                return new LocationQueryResponse()
                {
                    Error = "Natural language query cannot be empty."
                };
            }

            var queryParams = await ExtractQueryParamsAsync(naturalLanguageQuery);
            
            bool hasMeaningfulFilters =
                queryParams.MinLatitude.HasValue ||
                queryParams.MaxLatitude.HasValue ||
                queryParams.MinLongitude.HasValue ||
                queryParams.MaxLongitude.HasValue ||
                queryParams.LastAccessedAfter.HasValue ||
                queryParams.LastAccessedBefore.HasValue ||
                !string.IsNullOrEmpty(queryParams.OrderBy);

            if (hasMeaningfulFilters)
            {
                IQueryable<Location> query = _dbContext.Location;

                if (queryParams.MinLatitude.HasValue)
                {
                    query = query.Where(l => l.Latitude >= queryParams.MinLatitude.Value);
                }
                if (queryParams.MaxLatitude.HasValue)
                {
                    query = query.Where(l => l.Latitude <= queryParams.MaxLatitude.Value);
                }
                if (queryParams.MinLongitude.HasValue)
                {
                    query = query.Where(l => l.Longitude >= queryParams.MinLongitude.Value);
                }
                if (queryParams.MaxLongitude.HasValue)
                {
                    query = query.Where(l => l.Longitude <= queryParams.MaxLongitude.Value);
                }
                if (queryParams.LastAccessedAfter.HasValue)
                {
                    query = query.Where(l => l.AccessedDateTime >= queryParams.LastAccessedAfter.Value);
                }
                if (queryParams.LastAccessedBefore.HasValue)
                {
                    query = query.Where(l => l.AccessedDateTime <= queryParams.LastAccessedBefore.Value);
                }

                if (!string.IsNullOrEmpty(queryParams.OrderBy))
                {
                    switch (queryParams.OrderBy.ToLower())
                    {
                        case "accesseddatetime_desc":
                            query = query.OrderByDescending(l => l.AccessedDateTime);
                            break;
                        case "accesseddatetime_asc":
                            query = query.OrderBy(l => l.AccessedDateTime);
                            break;
                        case "latitude_asc":
                            query = query.OrderBy(l => l.Latitude);
                            break;
                        case "latitude_desc":
                            query = query.OrderByDescending(l => l.Latitude);
                            break;
                        default:
                            break;
                    }
                }

                if (queryParams.Limit.HasValue && queryParams.Limit.Value > 0)
                {
                    query = query.Take(queryParams.Limit.Value);
                }

                List<Location> locations = await query.ToListAsync();

                string summary = await SummarizeLocationsAsync(locations, naturalLanguageQuery);
                return new LocationQueryResponse()
                {
                    Summary = summary,
                    Locations = locations,
                    RawCount = locations.Count
                };
            }
            else
            {
                return new LocationQueryResponse()
                {
                    Error = "Input provided is not ideal for retrieving locations, please provide more meaningful input"
                };
            } 
        }
    }
}

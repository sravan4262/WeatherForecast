public interface IAzureOpenAIService
{
    Task<LocationQueryResponse> AskLocationsAsync(string userQuestion);
}
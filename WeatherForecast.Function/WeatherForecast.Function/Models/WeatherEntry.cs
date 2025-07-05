// WeatherForecast.Function/Models/WeatherEntry.cs
using System;
using Newtonsoft.Json; // For JSON serialization/deserialization

namespace WeatherForecast.Function.Models
{
    public class WeatherEntry
    {
        [JsonProperty("id")] 
        public string Id { get; set; } = Guid.NewGuid().ToString(); 

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("temperatureC")]
        public int TemperatureC { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("partitionKey")] // Recommended for Cosmos DB for good performance
        public string PartitionKey { get; set; } // Often same as City or a logical grouping
    }
}
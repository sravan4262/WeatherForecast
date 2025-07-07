using WeatherForecast.Domain.Entities;

public class LocationEvent
{
    public string EventType { get; set; } // e.g., "LocationInserted" or "LocationUpdated"
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Location Data { get; set; }
}

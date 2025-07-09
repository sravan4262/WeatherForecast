using WeatherForecast.Domain.Entities;

public class LocationQueryResponse
    {
        public string Summary { get; set; }

        public List<Location> Locations { get; set; }

        public int RawCount { get; set; }

        public string Error { get; set; }
    }
namespace WeatherForecast.Areas.Location.V1.ViewModels
{
    public class LocationViewModel
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime AccessedDateTime { get; set; }
    }
}

namespace WeatherForecast.Areas.WeatherForecast.V1.ViewModels
{
    public class WeatherForecastViewModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public int WeatherCode { get; set; }
        public DateTime Time { get; set; }
        public string Timezone { get; set; }
        public string WeatherDescription { get; set; }
    }
}

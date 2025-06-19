using WeatherForecast.Domain.OpenMeteo;

namespace WeatherForecast.Areas.WeatherForecast.V1.Mappers
{
    public class WeatherViewModelMapper : IWeatherViewModelMapper
    {
        public ViewModels.WeatherForecastViewModel Map(OpenMeteoResponse openMeteoResponse)
        {
            return new ViewModels.WeatherForecastViewModel
            {
                Latitude = openMeteoResponse.Latitude,
                Longitude = openMeteoResponse.Longitude,
                Temperature = openMeteoResponse.Current_weather.Temperature,
                WindSpeed = openMeteoResponse.Current_weather.Windspeed,
                WeatherCode = openMeteoResponse.Current_weather.Weathercode,
                Timezone = openMeteoResponse.Timezone 
            };
        }
    }
}

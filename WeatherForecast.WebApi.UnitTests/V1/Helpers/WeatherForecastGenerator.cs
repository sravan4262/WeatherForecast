using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Areas.WeatherForecast.V1.ViewModels;
using WeatherForecast.Domain.OpenMeteo;

namespace WeatherForecast.WebApi.UnitTests.V1.Helpers
{
    public static class WeatherForecastGenerator
    {
        public static OpenMeteoResponse DefaultOpenMeteoResponse =>
            new OpenMeteoResponse
            {
                Current_weather = new CurrentWeather
                {
                    Temperature = 25.5,
                    Windspeed = 10.0,
                    Weathercode = 800
                },
                Timezone = "EST"
            };

        public static WeatherForecastViewModel DefaultWeatherForecastViewModel =>
            new WeatherForecastViewModel
            {
                Temperature = 25.5,
                WindSpeed = 10.0,
                WeatherCode = 800,
                Timezone = "EST",
            };
    }
}

using WeatherForecast.Domain.OpenMeteo;

namespace WeatherForecast.Areas.WeatherForecast.V1.Mappers
{
    public interface IWeatherViewModelMapper
    {
        ViewModels.WeatherForecastViewModel Map(OpenMeteoResponse openMeteoResponse);
    }
}

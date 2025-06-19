using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherForecast.Domain.Business.Interfaces;
using WeatherForecast.Domain.DataAccess.Interfaces;
using WeatherForecast.Domain.OpenMeteo;

namespace WeatherForecast.Domain.Business.Classes
{
    public class WeatherRetriever : IWeatherRetriever
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IWeatherForecastRepository _weatherForecastRepository;
        private readonly HttpClient _httpClient;
        const string ExceptionMessage = "Weather Data retrieval failed.";

        public WeatherRetriever(HttpClient httpClient, ILocationRepository locationRepository, IWeatherForecastRepository weatherForecastRepository)
        {
            _httpClient = httpClient;
            _locationRepository = locationRepository;
            _weatherForecastRepository = weatherForecastRepository;
        }

        public async Task<OpenMeteoResponse> GetWeatherForecastAsync(double latitude, double longitude, bool insertLocation = true)
        {
            if (latitude == 0)
            {
                throw new ArgumentNullException(nameof(latitude));
            }
            if (longitude == 0)
            {
                throw new ArgumentNullException(nameof(longitude));
            }

            if (insertLocation)
            {
                // To Insert everytime user searches for a location
                await _locationRepository.InsertLocationAsync(new Entities.Location()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    AccessedDateTime = DateTime.UtcNow
                });
            }

            var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true&timezone=auto";
            var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
                throw new Exception(ExceptionMessage);
            var stringContent = await response.Content.ReadAsStringAsync();

            var openMeteoResponse = JsonSerializer.Deserialize<OpenMeteoResponse>(stringContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (openMeteoResponse?.Current_weather == null)
            {
                throw new Exception("Data not available from Open Meteo");
            }

            return openMeteoResponse;
        }

        public async Task<OpenMeteoResponse> GetWeatherForecastByLocationIdAsync(int locationId)
        {
            var location = await _locationRepository.GetLocationAsyncById(locationId);
            if (location == null)
            {
                throw new Exception("Location not found.");
            }
            return await GetWeatherForecastAsync(location.Latitude, location.Longitude);
        }

        public async Task<int> AddAsync(double latitude, double longitude)
        {
            var locationId = await _locationRepository.GetLocationIdByLatitudeLongiture(latitude, longitude);
            var insertLocation = true;
            if(locationId <= 0 || locationId is null)
            {
                insertLocation = false;
                locationId = 
                await _locationRepository.InsertLocationAsync(new Entities.Location()
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    AccessedDateTime = DateTime.UtcNow
                });
            }

            var openMateoResponse = await GetWeatherForecastAsync(latitude, longitude, insertLocation);

            var weatherForecast = new Entities.WeatherForecast()
            {
                Temperature = openMateoResponse.Current_weather.Temperature,
                WindSpeed = openMateoResponse.Current_weather.Windspeed,
                WeatherCode = openMateoResponse.Current_weather.Weathercode,
                Timezone = openMateoResponse.Timezone,
                LocationId = (int)locationId
            };
            return await _weatherForecastRepository.InsertWeatherForecastAsync(weatherForecast);

        }
    }
}

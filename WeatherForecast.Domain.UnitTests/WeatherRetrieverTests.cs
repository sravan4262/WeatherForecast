using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WeatherForecast.Domain.Business.Classes;
using WeatherForecast.Domain.DataAccess.Interfaces;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.OpenMeteo;

namespace WeatherForecast.Domain.UnitTests
{
    [TestFixture]
    public class WeatherRetrieverTests
    {
        private WeatherRetriever _weatherRetriever;
        private Mock<ILocationRepository> _locationRepositoryMock;
        private Mock<IWeatherForecastRepository> _weatherForecastRepositoryMock;
        private Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private HttpClient _httpClient;

        [SetUp]
        public void Setup()
        {
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _weatherForecastRepositoryMock = new Mock<IWeatherForecastRepository>();
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("https://api.open-meteo.com/")
            };

            _weatherRetriever = new WeatherRetriever(_httpClient, _locationRepositoryMock.Object, _weatherForecastRepositoryMock.Object);
        }

        private void SetupHttpClientSuccessResponse(OpenMeteoResponse responseContent)
        {
            var jsonResponse = JsonSerializer.Serialize(responseContent);
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
                });
        }

        private void SetupHttpClientFailureResponse(HttpStatusCode statusCode)
        {
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent("Error")
                });
        }


        [Test]
        public void ShouldThrowArgumentNullException_WhenLatitudeIsZero()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _weatherRetriever.GetWeatherForecastAsync(0, 10.0));
        }

        [Test]
        public void ShouldThrowArgumentNullException_WhenLongitudeIsZero()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _weatherRetriever.GetWeatherForecastAsync(10.0, 0));
        }

        [Test]
        public async Task ShouldInsertLocation_WhenInsertLocationIsTrue()
        {
            double latitude = 34.0;
            double longitude = -118.0;
            SetupHttpClientSuccessResponse(new OpenMeteoResponse
            {
                Current_weather = new CurrentWeather { Temperature = 25.5, Windspeed = 10.0, Weathercode = 1 },
                Timezone = "GMT"
            });
            _locationRepositoryMock.Setup(x => x.InsertLocationAsync(It.IsAny<Location>())).ReturnsAsync(1);

            await _weatherRetriever.GetWeatherForecastAsync(latitude, longitude, true);

            _locationRepositoryMock.Verify(repo => repo.InsertLocationAsync(It.Is<Location>(l =>
                l.Latitude == latitude && l.Longitude == longitude)), Times.Once);
        }

        [Test]
        public async Task ShouldNotInsertLocation_WhenInsertLocationIsFalse()
        {
            double latitude = 34.0;
            double longitude = -118.0;
            SetupHttpClientSuccessResponse(new OpenMeteoResponse
            {
                Current_weather = new CurrentWeather { Temperature = 25.5, Windspeed = 10.0, Weathercode = 1 },
                Timezone = "GMT"
            });

            await _weatherRetriever.GetWeatherForecastAsync(latitude, longitude, false);

            _locationRepositoryMock.Verify(repo => repo.InsertLocationAsync(It.IsAny<Location>()), Times.Never);
        }


        [Test]
        public async Task ShouldReturnOpenMeteoResponse_OnSuccess()
        {
            double latitude = 34.0;
            double longitude = -118.0;
            var expectedResponse = new OpenMeteoResponse
            {
                Current_weather = new CurrentWeather { Temperature = 25.5, Windspeed = 10.0, Weathercode = 1 },
                Timezone = "GMT"
            };
            SetupHttpClientSuccessResponse(expectedResponse);
            _locationRepositoryMock.Setup(x => x.InsertLocationAsync(It.IsAny<Location>())).ReturnsAsync(1);

            var result = await _weatherRetriever.GetWeatherForecastAsync(latitude, longitude);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Current_weather.Temperature, Is.EqualTo(expectedResponse.Current_weather.Temperature));
            Assert.That(result.Current_weather.Windspeed, Is.EqualTo(expectedResponse.Current_weather.Windspeed));
            Assert.That(result.Current_weather.Weathercode, Is.EqualTo(expectedResponse.Current_weather.Weathercode));
            Assert.That(result.Timezone, Is.EqualTo(expectedResponse.Timezone));
        }

        [Test]
        public void ShouldThrowException_WhenLocationNotFound()
        {
            int locationId = 99;
            _locationRepositoryMock.Setup(x => x.GetLocationAsyncById(locationId)).ReturnsAsync((Location?)null);

            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _weatherRetriever.GetWeatherForecastByLocationIdAsync(locationId));
            Assert.That(ex.Message, Is.EqualTo("Location not found."));
        }

        [Test]
        public async Task ShouldCallGetLocationAsyncById()
        {
            int locationId = 1;
            var location = new Location { Id = locationId, Latitude = 34.0, Longitude = -118.0 };
            _locationRepositoryMock.Setup(x => x.GetLocationAsyncById(locationId)).ReturnsAsync(location);
            SetupHttpClientSuccessResponse(new OpenMeteoResponse
            {
                Current_weather = new CurrentWeather { Temperature = 25.5, Windspeed = 10.0, Weathercode = 1 },
                Timezone = "GMT"
            });
            _locationRepositoryMock.Setup(x => x.InsertLocationAsync(It.IsAny<Location>())).ReturnsAsync(1);

            await _weatherRetriever.GetWeatherForecastByLocationIdAsync(locationId);

            _locationRepositoryMock.Verify(repo => repo.GetLocationAsyncById(locationId), Times.Once);
        }

        [Test]
        public async Task ShouldCheckForExistingLocation()
        {
            double latitude = 34.0;
            double longitude = -118.0;
            _locationRepositoryMock.Setup(x => x.GetLocationIdByLatitudeLongiture(latitude, longitude)).ReturnsAsync(1);
            SetupHttpClientSuccessResponse(new OpenMeteoResponse
            {
                Current_weather = new CurrentWeather { Temperature = 25.5, Windspeed = 10.0, Weathercode = 1 },
                Timezone = "GMT"
            });
            _weatherForecastRepositoryMock.Setup(x => x.InsertWeatherForecastAsync(It.IsAny<WeatherForecast.Domain.Entities.WeatherForecast>())).ReturnsAsync(1);

            await _weatherRetriever.AddAsync(latitude, longitude);

            _locationRepositoryMock.Verify(repo => repo.GetLocationIdByLatitudeLongiture(latitude, longitude), Times.Once);
        }

        [Test]
        public async Task ShouldCallInsertWeatherForecastAsync_WithCorrectData()
        {
            double latitude = 34.0;
            double longitude = -118.0;
            int existingLocationId = 10;
            var openMeteoResponse = new OpenMeteoResponse
            {
                Current_weather = new CurrentWeather { Temperature = 25.5, Windspeed = 10.0, Weathercode = 1 },
                Timezone = "GMT"
            };

            _locationRepositoryMock.Setup(x => x.GetLocationIdByLatitudeLongiture(latitude, longitude)).ReturnsAsync(existingLocationId);
            SetupHttpClientSuccessResponse(openMeteoResponse);
            _weatherForecastRepositoryMock.Setup(x => x.InsertWeatherForecastAsync(It.IsAny<WeatherForecast.Domain.Entities.WeatherForecast>())).ReturnsAsync(1);

            await _weatherRetriever.AddAsync(latitude, longitude);
            _weatherForecastRepositoryMock.Verify(repo => repo.InsertWeatherForecastAsync(It.Is<WeatherForecast.Domain.Entities.WeatherForecast>(wf =>
                wf.Temperature == openMeteoResponse.Current_weather.Temperature &&
                wf.WindSpeed == openMeteoResponse.Current_weather.Windspeed &&
                wf.WeatherCode == openMeteoResponse.Current_weather.Weathercode &&
                wf.Timezone == openMeteoResponse.Timezone &&
                wf.LocationId == existingLocationId
            )), Times.Once);
        }

    }
}

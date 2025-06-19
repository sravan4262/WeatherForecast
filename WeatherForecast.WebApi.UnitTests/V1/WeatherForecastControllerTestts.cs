using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Areas.WeatherForecast.V1.Controllers;
using WeatherForecast.Areas.WeatherForecast.V1.Mappers;
using WeatherForecast.Areas.WeatherForecast.V1.ViewModels;
using WeatherForecast.Domain.Business.Interfaces;
using WeatherForecast.Domain.OpenMeteo;
using WeatherForecast.WebApi.UnitTests.V1.Helpers;

namespace WeatherForecast.WebApi.UnitTests.V1
{
    [TestFixture]
    public class WeatherForecastControllerTests
    {
        private Mock<IWeatherRetriever> _weatherRetrieverMock;
        private Mock<IWeatherUpserter> _weatherUpserterMock;
        private Mock<IWeatherViewModelMapper> _weatherViewModelMapperMock;

        private WeatherForecastController _weatherForecastController;

        [SetUp]
        public void Setup()
        {
            _weatherRetrieverMock = new Mock<IWeatherRetriever>();
            _weatherUpserterMock = new Mock<IWeatherUpserter>();
            _weatherViewModelMapperMock = new Mock<IWeatherViewModelMapper>();

            _weatherForecastController = new WeatherForecastController(
                _weatherRetrieverMock.Object,
                _weatherViewModelMapperMock.Object,
                _weatherUpserterMock.Object);
        }

        [Test]
        public async Task ShouldReturnOkResult_WithWeatherForecastViewModel_OnSuccess()
        {
            int locationId = 1;
            var openMeteoResponse = WeatherForecastGenerator.DefaultOpenMeteoResponse;
            var viewModel = WeatherForecastGenerator.DefaultWeatherForecastViewModel;

            _weatherRetrieverMock.Setup(x => x.GetWeatherForecastByLocationIdAsync(locationId))
                .ReturnsAsync(openMeteoResponse);
            _weatherViewModelMapperMock.Setup(x => x.Map(openMeteoResponse))
                .Returns(viewModel);

            var actionResult = await _weatherForecastController.GetForecastByLocation(locationId);

            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));           

            _weatherRetrieverMock.Verify(x => x.GetWeatherForecastByLocationIdAsync(locationId), Times.Once);
            _weatherViewModelMapperMock.Verify(x => x.Map(openMeteoResponse), Times.Once);
        }

        [Test]
        public void ShouldThrowException_WhenRetrieverFails()
        {
            int locationId = 99;
            _weatherRetrieverMock.Setup(x => x.GetWeatherForecastByLocationIdAsync(locationId))
                .ThrowsAsync(new Exception("Location not found or external API error."));

            Assert.ThrowsAsync<Exception>(async () =>
                await _weatherForecastController.GetForecastByLocation(locationId));

            _weatherRetrieverMock.Verify(x => x.GetWeatherForecastByLocationIdAsync(locationId), Times.Once);
            _weatherViewModelMapperMock.Verify(x => x.Map(It.IsAny<OpenMeteoResponse>()), Times.Never);
        }


        [Test]
        public async Task ShouldReturnOkResult_WithWeatherForecastViewModelOnSuccess()
        {
            double latitude = 39.7391;
            double longitude = -75.5398;
            var openMeteoResponse = WeatherForecastGenerator.DefaultOpenMeteoResponse;
            var viewModel = WeatherForecastGenerator.DefaultWeatherForecastViewModel;

            _weatherRetrieverMock.Setup(x => x.GetWeatherForecastAsync(latitude, longitude, true))
                .ReturnsAsync(openMeteoResponse);
            _weatherViewModelMapperMock.Setup(x => x.Map(openMeteoResponse))
                .Returns(viewModel);

            var actionResult = await _weatherForecastController.GetForecastByLatitudeLongitude(latitude, longitude);

            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));           

            _weatherRetrieverMock.Verify(x => x.GetWeatherForecastAsync(latitude, longitude, true), Times.Once);
            _weatherViewModelMapperMock.Verify(x => x.Map(openMeteoResponse), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenRetrieverFails()
        {
            double latitude = 0;
            double longitude = 10.0;
            _weatherRetrieverMock.Setup(x => x.GetWeatherForecastAsync(latitude, longitude, true))
                .ThrowsAsync(new ArgumentNullException(nameof(latitude)));

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _weatherForecastController.GetForecastByLatitudeLongitude(latitude, longitude));

            _weatherRetrieverMock.Verify(x => x.GetWeatherForecastAsync(latitude, longitude, true), Times.Once);
            _weatherViewModelMapperMock.Verify(x => x.Map(It.IsAny<OpenMeteoResponse>()), Times.Never);
        }     

        [Test]
        public async Task ShouldReturnOkResult_WithWeatherId_OnSuccess()
        {
            double latitude = 39.7391;
            double longitude = -75.5398;
            int expectedWeatherId = 123;

            _weatherRetrieverMock.Setup(x => x.AddAsync(latitude, longitude))
                .ReturnsAsync(expectedWeatherId);

            var actionResult = await _weatherForecastController.Insert(latitude, longitude);

            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedWeatherId));

            _weatherRetrieverMock.Verify(x => x.AddAsync(latitude, longitude), Times.Once);
        }       

        [Test]
        public async Task ShouldReturnOkResult_OnSuccessfulDeletion()
        {
            int idToDelete = 1;
            _weatherUpserterMock.Setup(x => x.DeleteAsync(idToDelete))
                .Returns(Task.CompletedTask);

            var actionResult = await _weatherForecastController.DeleteWeatherForecast(idToDelete);

            Assert.That(actionResult, Is.InstanceOf<OkResult>());
            var okResult = (OkResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            _weatherUpserterMock.Verify(x => x.DeleteAsync(idToDelete), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionWhenUpserterFails()
        {
            int invalidId = -5;
            _weatherUpserterMock.Setup(x => x.DeleteAsync(invalidId))
                .ThrowsAsync(new ArgumentNullException(nameof(invalidId)));

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _weatherForecastController.DeleteWeatherForecast(invalidId));

            _weatherUpserterMock.Verify(x => x.DeleteAsync(invalidId), Times.Once);
        }
    }
}

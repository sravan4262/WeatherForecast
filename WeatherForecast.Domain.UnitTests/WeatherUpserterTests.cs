using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Business.Classes;
using WeatherForecast.Domain.DataAccess.Interfaces;

namespace WeatherForecast.Domain.UnitTests
{
    [TestFixture]
    public class WeatherUpserterTests
    {
        private WeatherUpserter _weatherUpserter;
        private Mock<IWeatherForecastRepository> _weatherForecastRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _weatherForecastRepositoryMock = new Mock<IWeatherForecastRepository>();
            _weatherForecastRepositoryMock.Setup(x => x.DeleteWeatherForecstAsyncById(It.IsAny<int>())).Returns(Task.CompletedTask);

            _weatherUpserter = new WeatherUpserter(_weatherForecastRepositoryMock.Object);
        }

        [Test]
        public void ShouldThrowArgumentNullException_WhenIdIsNegative()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _weatherUpserter.DeleteAsync(-1));
        }

        [Test]
        public async Task ShouldCallDeleteWeatherForecastAsyncById_OnceWithCorrectId()
        {
            int idToDelete = 5;
            await _weatherUpserter.DeleteAsync(idToDelete);
            _weatherForecastRepositoryMock.Verify(repo => repo.DeleteWeatherForecstAsyncById(idToDelete), Times.Once);
        }
    }
}

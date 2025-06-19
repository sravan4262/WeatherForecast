using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Business.Classes;
using WeatherForecast.Domain.DataAccess.Interfaces;
using WeatherForecast.Domain.UnitTests.Helpers;

namespace WeatherForecast.Domain.UnitTests
{
    [TestFixture]
    public class LocationRetrieverTests
    {
        private LocationRetriever _locationRetriever;
        private Mock<ILocationRepository> _locationRepositoryMock;
        [SetUp]
        public void Setup()
        {
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _locationRepositoryMock.Setup(x => x.GetLocationAsyncById(It.IsAny<int>())).ReturnsAsync(LocationHelper.Default);
            _locationRepositoryMock.Setup(x => x.GetLocationsAsync()).ReturnsAsync(LocationHelper.DefaultList);

            _locationRetriever = new LocationRetriever(_locationRepositoryMock.Object);
        }

        [Test]
        public void ShouldThrowExceptionIfIdIsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                   await _locationRetriever.GetLocationAsyncById(-1));
        }

        [Test]
        public async Task ShouldGetLocationById()
        {
            await _locationRetriever.GetLocationAsyncById(1);
            _locationRepositoryMock.Verify(repo => repo.GetLocationAsyncById(It.IsAny<int>()), Times.Once);
        }

        [Test]
        public async Task ShouldGetAllLocations()
        {
            await _locationRetriever.GetLocationsAsync();
            _locationRepositoryMock.Verify(repo => repo.GetLocationsAsync(), Times.Once);
        }
    }
}

using Moq;
using NUnit.Framework;
using WeatherForecast.Domain.Business.Classes;
using WeatherForecast.Domain.DataAccess.Interfaces;
using WeatherForecast.Domain.Entities;
using WeatherForecast.Domain.UnitTests.Helpers;

namespace WeatherForecast.Domain.UnitTests
{
    [TestFixture]
    public class LocationUpserterTests
    {
        private LocationUpserter _locationUpserter;
        private Mock<ILocationRepository> _locationRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _locationRepositoryMock = new Mock<ILocationRepository>();
            _locationRepositoryMock.Setup(x => x.InsertLocationAsync(It.IsAny<Location>())).ReturnsAsync(1); // Assuming 1 for successful add
            _locationRepositoryMock.Setup(x => x.UpdateLocationAsync(It.IsAny<Location>())).ReturnsAsync(1); // Assuming 1 for successful update
            _locationRepositoryMock.Setup(x => x.DeleteLocationAsyncById(It.IsAny<int>())).Returns(Task.CompletedTask);

            _locationUpserter = new LocationUpserter(_locationRepositoryMock.Object);
        }


        [Test]
        public void ShouldThrowArgumentNullException_WhenLocationIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _locationUpserter.AddAsync(null));
        }

        [Test]
        public async Task ShouldCallInsertLocationAsync_OnceWithCorrectLocation()
        {
            var locationToAdd = LocationHelper.Default();

            await _locationUpserter.AddAsync(locationToAdd);

            _locationRepositoryMock.Verify(repo => repo.InsertLocationAsync(locationToAdd), Times.Once);
        }

        [Test]
        public async Task ShouldReturnInsertedLocationId()
        {
            var locationToAdd = LocationHelper.Default();
            _locationRepositoryMock.Setup(x => x.InsertLocationAsync(locationToAdd)).ReturnsAsync(10);

            var resultId = await _locationUpserter.AddAsync(locationToAdd);

            Assert.That(resultId, Is.EqualTo(10));
        }


    [Test]
        public void ShouldThrowArgumentNullException_WhenIdIsNegative()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _locationUpserter.DeleteAsync(-1));
        }

        [Test]
        public async Task ShouldCallDeleteLocationAsyncById_OnceWithCorrectId()
        {
            int idToDelete = 5;

            await _locationUpserter.DeleteAsync(idToDelete);

            _locationRepositoryMock.Verify(repo => repo.DeleteLocationAsyncById(idToDelete), Times.Once);
        }

    [Test]
        public void ShouldThrowArgumentNullException_WhenLocationIsNullWhenUpdate()
        {
            Assert.ThrowsAsync<ArgumentNullException>(async () => await _locationUpserter.UpdateAsync(null));
        }

        [Test]
        public async Task ShouldCallUpdateLocationAsync_OnceWithCorrectLocation()
        {
            var locationToUpdate = LocationHelper.Default();

            await _locationUpserter.UpdateAsync(locationToUpdate);

            _locationRepositoryMock.Verify(repo => repo.UpdateLocationAsync(locationToUpdate), Times.Once);
        }

        [Test]
        public async Task ShouldReturnUpdatedLocationId()
        {
            var locationToUpdate = LocationHelper.Default();
            _locationRepositoryMock.Setup(x => x.UpdateLocationAsync(locationToUpdate)).ReturnsAsync(locationToUpdate.Id);

            var resultId = await _locationUpserter.UpdateAsync(locationToUpdate);
            Assert.That(resultId, Is.EqualTo(locationToUpdate.Id));
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;

using WeatherForecast.Areas.Location.V1.Controllers;
using WeatherForecast.Areas.Location.V1.Mappers;
using WeatherForecast.Areas.Location.V1.ViewModels;
using WeatherForecast.Domain.Business.Interfaces;
using WeatherForecast.WebApi.UnitTests.V1.Helpers;

namespace WeatherForecast.WebApi.UnitTests.V1
{
    [TestFixture]
    public class LocationControllerTests
    {
        private Mock<ILocationRetriever> _locationRetrieverMock;
        private Mock<ILocationUpserter> _locationUpserterMock;
        private Mock<ILocationViewModelMapper> _viewModelMapperMock;

        private LocationController _locationController;

        [SetUp]
        public void Setup()
        {
            _locationRetrieverMock = new Mock<ILocationRetriever>();
            _locationUpserterMock = new Mock<ILocationUpserter>();
            _viewModelMapperMock = new Mock<ILocationViewModelMapper>();

            _locationController = new LocationController(
                _locationRetrieverMock.Object,
                _locationUpserterMock.Object,
                _viewModelMapperMock.Object);
        }

        [Test]
        public async Task ShouldReturnOkResult_WithListOfLocationViewModels()
        {
            var domainLocations = LocationGenerator.DefaultLocationList;
            var viewModels = LocationViewModelGenerator.DefaultLocationViewModelList;

            _locationRetrieverMock.Setup(x => x.GetLocationsAsync())
                .ReturnsAsync(domainLocations);
            _viewModelMapperMock.Setup(x => x.Map(domainLocations))
                .Returns(viewModels);

            var actionResult = await _locationController.GetLocations();

            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));            

            _locationRetrieverMock.Verify(x => x.GetLocationsAsync(), Times.Once);
            _viewModelMapperMock.Verify(x => x.Map(domainLocations), Times.Once);
        }

        [Test]
        public async Task ShouldReturnEmptyList_WhenNoLocationsExist()
        {
            var domainLocations = new List<Domain.Entities.Location>();
            var viewModels = new List<LocationViewModel>();

            _locationRetrieverMock.Setup(x => x.GetLocationsAsync())
                .ReturnsAsync(domainLocations);
            _viewModelMapperMock.Setup(x => x.Map(domainLocations))
                .Returns(viewModels);

            var actionResult = await _locationController.GetLocations();

            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            _locationRetrieverMock.Verify(x => x.GetLocationsAsync(), Times.Once);
            _viewModelMapperMock.Verify(x => x.Map(domainLocations), Times.Once);
        }

        [Test]
        public void ShouldHandleException_AndReturn500()
        {
            _locationRetrieverMock.Setup(x => x.GetLocationsAsync())
                .ThrowsAsync(new Exception("Database error"));

            Assert.ThrowsAsync<Exception>(async () => await _locationController.GetLocations());

            _locationRetrieverMock.Verify(x => x.GetLocationsAsync(), Times.Once);
            _viewModelMapperMock.Verify(x => x.Map(It.IsAny<List<Domain.Entities.Location>>()), Times.Never);
        }


        [Test]
        public async Task ShouldReturnOkResult_WithLocationViewModel_WhenLocationExists()
        {
            int locationId = 1;
            var domainLocation = LocationGenerator.DefaultLocation;
            var viewModel = LocationViewModelGenerator.DefaultLocationViewModel;

            _locationRetrieverMock.Setup(x => x.GetLocationAsyncById(locationId))
                .ReturnsAsync(domainLocation);
            _viewModelMapperMock.Setup(x => x.Map(domainLocation))
                .Returns(viewModel);

            var actionResult = await _locationController.GetLocations(locationId);

            // Assert
            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));           

            _locationRetrieverMock.Verify(x => x.GetLocationAsyncById(locationId), Times.Once);
            _viewModelMapperMock.Verify(x => x.Map(domainLocation), Times.Once);
        }


        [Test]
        public async Task ShouldReturnOkResult_WithNewLocationId_OnSuccess()
        {
            var insertRequestModel = LocationGenerator.DefaultInsertRequestModel;
            var mappedLocation = new Domain.Entities.Location {Latitude = insertRequestModel.Latitude, Longitude = insertRequestModel.Longitude };
            int expectedId = 5;

            _viewModelMapperMock.Setup(x => x.Map(insertRequestModel))
                .Returns(mappedLocation);
            _locationUpserterMock.Setup(x => x.AddAsync(mappedLocation))
                .ReturnsAsync(expectedId);

            var actionResult = await _locationController.AddLocation(insertRequestModel);

            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedId));

            _viewModelMapperMock.Verify(x => x.Map(insertRequestModel), Times.Once);
            _locationUpserterMock.Verify(x => x.AddAsync(mappedLocation), Times.Once);
        }

        [Test]
        public void ShouldHandleArgumentNullException_ForInvalidModel()
        {
            _viewModelMapperMock.Setup(x => x.Map(It.IsAny<LocationInsertRequestModel>()))
                                .Throws(new ArgumentNullException("insertRequestModel"));

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _locationController.AddLocation(new LocationInsertRequestModel()));

            _viewModelMapperMock.Verify(x => x.Map(It.IsAny<LocationInsertRequestModel>()), Times.Once);
            _locationUpserterMock.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Location>()), Times.Never);
        }
       

        [Test]
        public async Task ShouldReturnOkResult_WithUpdatedLocationId_OnSuccess()
        {
            // Arrange
            var updateRequestModel = LocationGenerator.DefaultUpdateRequestModel;
            var mappedLocation = new Domain.Entities.Location { Id = updateRequestModel.Id, Latitude = updateRequestModel.Latitude, Longitude = updateRequestModel.Longitude };
            int expectedId = updateRequestModel.Id;

            _viewModelMapperMock.Setup(x => x.Map(updateRequestModel))
                .Returns(mappedLocation);
            _locationUpserterMock.Setup(x => x.UpdateAsync(mappedLocation))
                .ReturnsAsync(expectedId);

            var actionResult = await _locationController.UpdateLocation(updateRequestModel);

            Assert.That(actionResult, Is.InstanceOf<OkObjectResult>());
            var okResult = (OkObjectResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(expectedId));

            _viewModelMapperMock.Verify(x => x.Map(updateRequestModel), Times.Once);
            _locationUpserterMock.Verify(x => x.UpdateAsync(mappedLocation), Times.Once);
        }

        [Test]
        public void ShouldHandleArgumentNullExceptionForInvalidModel()
        {
            _viewModelMapperMock.Setup(x => x.Map(It.IsAny<LocationUpdateRequestModel>()))
                                .Throws(new ArgumentNullException("updateRequestModel"));

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _locationController.UpdateLocation(new LocationUpdateRequestModel()));

            _viewModelMapperMock.Verify(x => x.Map(It.IsAny<LocationUpdateRequestModel>()), Times.Once);
            _locationUpserterMock.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Location>()), Times.Never);
        }
       

        [Test]
        public async Task ShouldReturnOkResult_OnSuccessfulDeletion()
        {
            int idToDelete = 1;
            _locationUpserterMock.Setup(x => x.DeleteAsync(idToDelete))
                .Returns(Task.CompletedTask); 

            var actionResult = await _locationController.DeleteLocation(idToDelete);

            Assert.That(actionResult, Is.InstanceOf<OkResult>());
            var okResult = (OkResult)actionResult;
            Assert.That(okResult.StatusCode, Is.EqualTo(200));

            _locationUpserterMock.Verify(x => x.DeleteAsync(idToDelete), Times.Once);
        }

        [Test]
        public void ShouldHandleArgumentNullException_WhenIdIsInvalid()
        {
            int invalidId = -1; 
            _locationUpserterMock.Setup(x => x.DeleteAsync(invalidId))
                .ThrowsAsync(new ArgumentNullException(nameof(invalidId)));

            Assert.ThrowsAsync<ArgumentNullException>(async () =>
                await _locationController.DeleteLocation(invalidId));

            _locationUpserterMock.Verify(x => x.DeleteAsync(invalidId), Times.Once);
        }
    }
}

namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Locations;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Models.ViewModels.Locations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Data.Models;
    using Xunit;

    public class LocationsServiceTests : BaseServiceTests
    {
        private const string FirstTestLocationName = "Sofia";
        private const string SecondTestLocationName = "Plovdiv";
        private const string TestLocationAddress = "Bul. Bulgaria 101";
        private const int TestLocationDestinationId = 1;
        private const string TestLocationDestinationName = "Bulgaria";
        private const string TestLocationType = "Museum";
        private const string TestInvalidLocationType = "Invalid";

        private ILocationsService LocationsServiceMock =>
            this.ServiceProvider.GetRequiredService<ILocationsService>();

        [Fact]
        public async Task CreateAsyncCreatesLocation()
        {
            await this.AddTestingDestinationToDb();
            var locationCreateInputModel = this.GetTestingLocationCreateInputModel();
            var location = await this.LocationsServiceMock.CreateLocationAsync(locationCreateInputModel);

            var locationsRepository = this.DbContext.Locations.OrderBy(l => l.CreatedOn);
            Assert.Collection(locationsRepository,
                elem1 =>
                {
                    Assert.Equal(locationsRepository.Last().Id, location.Id);
                    Assert.Equal(locationsRepository.Last().Name, location.Name);
                    Assert.Equal(locationsRepository.Last().Address, location.Address);
                });
            Assert.Equal(1, this.DbContext.Locations.Count());
        }

        [Fact]
        public async Task CreateAsyncThrowsArgumentExceptionWhenLocationExists()
        {
            await this.AddTestingDestinationToDb();

            this.DbContext.Locations.Add(new Location()
            {
                Id = 1,
                Name = FirstTestLocationName,
                Address = TestLocationAddress,
                DestinationId = TestLocationDestinationId,
                LocationType = LocationType.Cafe,
            });
            await this.DbContext.SaveChangesAsync();

            var locationCreateInputModel = this.GetTestingLocationCreateInputModel();

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                 this.LocationsServiceMock.CreateLocationAsync(locationCreateInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.LocationExists, locationCreateInputModel.Name, locationCreateInputModel.DestinationId), exception.Message);
        }

        [Fact]
        public async Task CreateAsyncThrowsArgumentExceptionIfLocationTypeIsInvalid()
        {
            await this.AddTestingDestinationToDb();

            var locationCreateInputModel = this.GetTestingLocationCreateInputModel();
            locationCreateInputModel.Type = TestInvalidLocationType;

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.LocationsServiceMock.CreateLocationAsync(locationCreateInputModel));

            Assert.Equal(string.Format(ServicesDataConstants.InvalidLocationType, TestInvalidLocationType), exception.Message);
        }

        [Fact]
        public async Task CreateAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            var locationCreateInputModel = this.GetTestingLocationCreateInputModel();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.LocationsServiceMock.CreateLocationAsync(locationCreateInputModel));

            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, TestLocationDestinationId), exception.Message);
        }

        [Fact]
        public async Task GetAllLocationsAsyncReturnsAllLocations()
        {
            await this.AddTestingDestinationToDb();

            this.DbContext.Locations.Add(new Location
            {
                Id = 1,
                Name = FirstTestLocationName,
                Address = TestLocationAddress,
                Activities = new HashSet<Activity>(),
                DestinationId = TestLocationDestinationId,
                LocationType = LocationType.Museum,
            });
            this.DbContext.Locations.Add(new Location
            {
                Id = 2,
                Name = SecondTestLocationName,
                Address = TestLocationAddress,
                Activities = new HashSet<Activity>(),
                DestinationId = TestLocationDestinationId,
                LocationType = LocationType.Museum,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new LocationViewModel[]
            {
                new LocationViewModel
                {
                    Id = 1,
                    Name = FirstTestLocationName,
                    Address = TestLocationAddress,
                    DestinationName = TestLocationDestinationName,
                    LocationType = TestLocationType,
                    Activities = new HashSet<ActivityViewModel>()
                },
                new LocationViewModel
                {
                    Id = 2,
                    Name = SecondTestLocationName,
                    Address = TestLocationAddress,
                    DestinationName = TestLocationDestinationName,
                    LocationType = TestLocationType,
                    Activities = new HashSet<ActivityViewModel>()
                },
            };

            var actual = await this.LocationsServiceMock.GetAllLocationsAsync();

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                    Assert.Equal(expected[0].Address, elem1.Address);
                    Assert.Equal(expected[0].DestinationName, elem1.DestinationName);
                    Assert.Equal(expected[0].LocationType, elem1.LocationType);
                    Assert.Equal(expected[0].Activities, elem1.Activities);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                    Assert.Equal(expected[1].Address, elem2.Address);
                    Assert.Equal(expected[1].DestinationName, elem2.DestinationName);
                    Assert.Equal(expected[1].LocationType, elem2.LocationType);
                    Assert.Equal(expected[1].Activities, elem2.Activities);
                });
            Assert.Equal(expected.Length, actual.Length);
        }

        private async Task AddTestingDestinationToDb()
        {
            this.DbContext.Destinations.Add(new Destination
            {
                Name = TestLocationDestinationName,
                Id = TestLocationDestinationId
            });
            await this.DbContext.SaveChangesAsync();
        }

        private LocationCreateInputModel GetTestingLocationCreateInputModel()
        {
            return new LocationCreateInputModel
            {
                Name = FirstTestLocationName,
                Address = TestLocationAddress,
                DestinationId = TestLocationDestinationId,
                Type = TestLocationType,
            };
        }
    }
}

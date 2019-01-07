namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CloudinaryDotNet;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Models.ViewModels.Home;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using Xunit;

    public class DestinationsServiceTests : BaseServiceTests
    {
        private const int TestDestinationId = 1;
        private const string TestDestinationName = "SofiaTest";
        private const int SecondTestDestinationId = 2;
        private const string SecondTestDestinationName = "WashingtonTest";

        private const int TestCountryId = 1;
        private const string TestCountryName = "BulgariaTest";
        private const int SecondTestCountryId = 2;
        private const string SecondTestCountryName = "USATest";

        private const string TestImageUrl = "https://someurl.com";
        private const string TestImagePath = "Test.jpg";
        private const string TestImageContentType = "image/jpg";

        private const int TestActivityId = 1;
        private const string TestActivityName = "Test Activity 123";
        private const string TestActivityType = "Adventure";

        private const int SecondTestActivityId = 2;
        private const string SecondTestActivityName = "Secondd Activity";

        private const int TestRestaurantId = 1;
        private const string TestRestaurantName = "Test Restaurant 123";
        private const int SecondTestRestaurantId = 2;
        private const string SecondTestRestaurantName = "Secondd Restaurant";
        private const string TestRestaurantType = "Bar";

        private readonly DateTime starDate = DateTime.Now.Subtract(new TimeSpan(2, 0, 0, 0));
        private readonly DateTime endDate = DateTime.Now.AddDays(2);

        private IDestinationsService DestinationsServiceMock => this.ServiceProvider.GetRequiredService<IDestinationsService>();

        [Fact]
        public async Task GetAllDestinationsAsyncReturnsAllDestinations()
        {
            await this.AddTestingCountryToDb();

            this.DbContext.Destinations.Add(new Destination
            {
                Id = TestDestinationId,
                Name = TestDestinationName,
                CountryId = TestCountryId,
            });
            this.DbContext.Destinations.Add(new Destination
            {
                Id = SecondTestCountryId,
                Name = SecondTestCountryName,
                CountryId = TestCountryId,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new DestinationDetailsViewModel[]
            {
                new DestinationDetailsViewModel
                {
                    Id = TestDestinationId,
                    Name = TestDestinationName,
                    CountryName = TestCountryName,
                },
                new DestinationDetailsViewModel
                {
                    Id = SecondTestDestinationId,
                    Name = SecondTestCountryName,
                    CountryName = TestCountryName,
                },
            };

            var actual = await this.DestinationsServiceMock.GetAllDestinationsAsync();

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                    Assert.Equal(expected[0].CountryName, elem1.CountryName);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                    Assert.Equal(expected[1].CountryName, elem2.CountryName);
                });
            Assert.Equal(expected.Length, actual.Count());
        }

        [Fact]
        public async Task CreateAsyncThrowsNullReferenceExceptionIfLocationNotFound()
        {
            var invalidDestinationCreateInputModel = new DestinationCreateInputModel
            {
                Name = TestDestinationName,
                CountryId = TestCountryId,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.DestinationsServiceMock.CreateAsync(invalidDestinationCreateInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceCountryId, invalidDestinationCreateInputModel.CountryId), exception.Message);
        }

        [Fact]
        public async Task CreateAsyncAddsDestinationToDbContext()
        {
            await this.AddTestingCountryToDb();

            DestinationDetailsViewModel destinationDetailsViewModel;
            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var destinationCreateInputModel = new DestinationCreateInputModel()
                {
                    Name = TestDestinationName,
                    CountryId = TestCountryId,
                    Image = file,
                };

                destinationDetailsViewModel = await this.DestinationsServiceMock.CreateAsync(destinationCreateInputModel);
            }

            ApplicationCloudinary.DeleteImage(ServiceProvider.GetRequiredService<Cloudinary>(), destinationDetailsViewModel.Name);

            var activitiesDbSet = this.DbContext.Destinations.OrderBy(r => r.CreatedOn);

            Assert.Collection(activitiesDbSet,
                elem1 =>
                {
                    Assert.Equal(activitiesDbSet.Last().Id, destinationDetailsViewModel.Id);
                    Assert.Equal(activitiesDbSet.Last().Name, destinationDetailsViewModel.Name);
                    Assert.Equal(activitiesDbSet.Last().Country.Name, destinationDetailsViewModel.CountryName);
                });
        }

        [Fact]
        public async Task EditAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            await this.AddTestingDestinationToDb();
            var invalidDestinationEditViewModel = new DestinationEditViewModel()
            {
                Id = SecondTestDestinationId,
                Name = SecondTestDestinationName,
                CountryId = TestCountryId,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.DestinationsServiceMock.EditAsync(invalidDestinationEditViewModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, invalidDestinationEditViewModel.Id), exception.Message);
        }

        [Fact]
        public async Task EditAsyncThrowsNullReferenceExceptionIfCountryNotFound()
        {
            await this.AddTestingDestinationToDb();
            var invalidDestinationEditViewModel = new DestinationEditViewModel()
            {
                Id = TestDestinationId,
                Name = TestDestinationName,
                CountryId = TestCountryId,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.DestinationsServiceMock.EditAsync(invalidDestinationEditViewModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceCountryId, invalidDestinationEditViewModel.CountryId), exception.Message);
        }

        [Fact]
        public async Task EditAsyncEditsDestinationWhenImageStaysTheSame()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            this.DbContext.Countries.Add(new Country() { Id = 2, Name = "TestEditLocation" });
            await this.DbContext.SaveChangesAsync();

            var newName = SecondTestDestinationName;
            var newCountryId = 2;

            Assert.NotEqual(newName, this.DbContext.Destinations.Find(TestDestinationId).Name);
            Assert.NotEqual(newCountryId, this.DbContext.Destinations.Find(TestDestinationId).CountryId);

            var destinationEditViewModel = new DestinationEditViewModel()
            {
                Id = TestDestinationId,
                Name = newName,
                CountryId = newCountryId,
                NewImage = null,
            };

            await this.DestinationsServiceMock.EditAsync(destinationEditViewModel);

            Assert.Equal(newName, this.DbContext.Destinations.Find(TestDestinationId).Name);
            Assert.Equal(newCountryId, this.DbContext.Destinations.Find(TestDestinationId).CountryId);
        }

        [Fact]
        public async Task EditAsyncEditsDestinationsImage()
        {
            await this.AddTestingCountryToDb();

            this.DbContext.Destinations.Add(new Destination
            {
                Id = TestDestinationId,
                Name = TestDestinationName,
                CountryId = TestCountryId,
                ImageUrl = TestImageUrl,
            });
            await this.DbContext.SaveChangesAsync();

            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var destinationEditViewModel = new DestinationEditViewModel()
                {
                    Id = TestDestinationId,
                    Name = TestDestinationName,
                    CountryId = TestCountryId,
                    NewImage = file,
                };

                await this.DestinationsServiceMock.EditAsync(destinationEditViewModel);

                ApplicationCloudinary.DeleteImage(ServiceProvider.GetRequiredService<Cloudinary>(), destinationEditViewModel.Name);
            }

            Assert.NotEqual(TestImageUrl, this.DbContext.Destinations.Find(TestDestinationId).ImageUrl);
        }

        [Fact]
        public async Task GetViewModelByIdAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.DestinationsServiceMock.GetViewModelByIdAsync<DestinationDetailsViewModel>(SecondTestDestinationId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, SecondTestDestinationId), exception.Message);
        }

        [Fact]
        public async Task GetViewModelByIdAsyncReturnsCorrectViewModel()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            var expected = this.DbContext.Destinations.OrderBy(r => r.CreatedOn);
            var actual = await this.DestinationsServiceMock.GetViewModelByIdAsync<DestinationViewModel>(TestDestinationId);

            Assert.IsType<DestinationViewModel>(actual);
            Assert.Collection(expected,
                elem1 =>
                {
                    Assert.Equal(expected.First().Id, actual.Id);
                    Assert.Equal(expected.First().Name, actual.Name);
                    Assert.Equal(expected.First().Country.Name, actual.CountryName);
                });
        }

        [Fact]
        public async Task DeleteByIdAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.DestinationsServiceMock.DeleteByIdAsync(SecondTestDestinationId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, SecondTestDestinationId), exception.Message);
        }

        [Fact]
        public async Task DeleteByIdAsyncDeletesTheCorrectDestination()
        {
            await this.AddTestingCountryToDb();

            var destinationToDelete = new Destination
            {
                Id = 3,
                Name = "To delete",
                CountryId = TestCountryId,
                IsDeleted = false,
            };
            this.DbContext.Destinations.Add(destinationToDelete);
            this.DbContext.Destinations.Add(new Destination
            {
                Id = SecondTestDestinationId,
                Name = SecondTestDestinationName,
                CountryId = TestCountryId,
                IsDeleted = false,
            });
            this.DbContext.Destinations.Add(new Destination
            {
                Id = 4,
                Name = "Another",
                CountryId = TestCountryId,
                IsDeleted = false,
            });
            await this.DbContext.SaveChangesAsync();

            await this.DestinationsServiceMock.DeleteByIdAsync(destinationToDelete.Id);
            Assert.DoesNotContain(destinationToDelete, this.DbContext.Destinations);
        }

        [Fact]
        public async Task GetSearchResultAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.DestinationsServiceMock.GetSearchResultAsync(SecondTestDestinationId, this.starDate, this.endDate));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, SecondTestDestinationId), exception.Message);
        }

        [Fact]
        public async Task GetSearchResultAsyncReturnsCorrectSearchResultViewModel()
        {
            await SetDbForGetSearchResult();

            var expected = GetSearchResultExpected();
            var actual = await this.DestinationsServiceMock.GetSearchResultAsync(TestDestinationId, this.starDate, this.endDate);

            Assert.Equal(expected.DestinationName, actual.DestinationName);
            Assert.Equal(expected.StartDate, actual.StartDate);
            Assert.Equal(expected.EndDate, actual.EndDate);
            Assert.Equal(expected.Activities.Count(), actual.Activities.Count());
            Assert.Equal(expected.Restaurants.Count(), actual.Restaurants.Count());
        }

        private SearchResultViewModel GetSearchResultExpected()
        {
            return new SearchResultViewModel
            {
                DestinationName = TestDestinationName,
                StartDate = this.starDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture),
                EndDate = this.endDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture),
                Activities = new List<ActivityViewModel>
                {
                    new ActivityViewModel
                    {
                        Id = TestActivityId,
                        Name = TestActivityName,
                        DestinationId = TestDestinationId,
                        Date = this.starDate.AddDays(2),
                        Type = ActivityType.Hiking.ToString(),
                    },
                },
                Restaurants = new List<RestaurantViewModel>
                {
                    new RestaurantViewModel
                    {
                        Id = TestRestaurantId,
                        Name = TestRestaurantName,
                        DestinationId = TestDestinationId,
                        Type = RestaurantType.CoffeeShop.ToString(),
                    },
                    new RestaurantViewModel
                    {
                        Id = SecondTestRestaurantId,
                        Name = SecondTestRestaurantName,
                        DestinationId = TestDestinationId,
                        Type = RestaurantType.CasualDining.ToString(),
                    }
                }
            };
        }

        private async Task SetDbForGetSearchResult()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            this.DbContext.Activities.Add(new Activity
            {
                Id = TestActivityId,
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Date = this.starDate.AddDays(2),
                Type = ActivityType.Hiking,
            });
            this.DbContext.Activities.Add(new Activity
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                DestinationId = SecondTestDestinationId,
                Date = this.starDate.AddDays(1),
                Type = ActivityType.Adventure,
            });
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.CoffeeShop,
            });
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = SecondTestRestaurantId,
                Name = SecondTestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.CasualDining,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingDestinationToDb()
        {
            this.DbContext.Destinations.Add(new Destination
            {
                Id = TestDestinationId,
                Name = TestDestinationName,
                CountryId = TestCountryId,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingCountryToDb()
        {
            this.DbContext.Countries.Add(new Country
            {
                Id = TestCountryId,
                Name = TestCountryName,
            });
            await this.DbContext.SaveChangesAsync();
        }
    }
}

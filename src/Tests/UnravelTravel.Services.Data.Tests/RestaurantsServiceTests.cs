namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Enums;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using CloudinaryDotNet;
    using Xunit;

    public class RestaurantsServiceTests : BaseServiceTests
    {
        private const int TestDestinationId = 1;
        private const string TestDestinationName = "Bulgaria";
        private const int SecondTestDestinationId = 2;
        private const string SecondTestDestinationName = "USA";

        private const int TestRestaurantId = 1;
        private const string TestRestaurantName = "Test Restaurant 123";
        private const string TestRestaurantAddress = "bul. Bulgaria 102";
        private const string SecondTestRestaurantAddress = "15 Vitosha Str.";
        private const int TestRestaurantSeats = 102;
        private const int SecondTestRestaurantId = 2;
        private const string SecondTestRestaurantName = "Secondd Restaurant";
        private const string TestRestaurantType = "Bar";
        private const string TestSearchString = "Vitosha";

        private const string InvalidRestaurantType = "InvalidType";

        private const string TestImageUrl = "https://someurl.com";
        private const string TestImagePath = "Test.jpg";
        private const string TestImageContentType = "image/jpg";

        private const string TestUserName = "Pesho";
        private const string InvalidUsername = "Stamat";

        private const double TestReviewRating = 4.2;
        private const double SecondTestReviewRating = 1.2;
        private const string TestReviewContent = "Testing review.";

        private readonly string testUserId = Guid.NewGuid().ToString();

        private IRestaurantsService RestaurantsServiceMock =>
            this.ServiceProvider.GetRequiredService<IRestaurantsService>();

        [Fact]
        public async Task GetAllAsyncReturnsAllRestaurants()
        {
            await this.AddTestingDestinationToDb();
            this.DbContext.Destinations.Add(new Destination
            { Id = SecondTestDestinationId, Name = SecondTestDestinationName });
            await this.DbContext.SaveChangesAsync();

            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.Bar,
            });
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = SecondTestRestaurantId,
                Name = SecondTestRestaurantName,
                DestinationId = SecondTestDestinationId,
                Type = RestaurantType.Bar,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new RestaurantViewModel[]
            {
                new RestaurantViewModel
                {
                    Id = TestRestaurantId,
                    Name = TestRestaurantName,
                    DestinationId = TestDestinationId,
                    DestinationName = TestDestinationName,
                    Type = TestRestaurantType,
                },
                new RestaurantViewModel
                {
                    Id = SecondTestRestaurantId,
                    Name = SecondTestRestaurantName,
                    DestinationId = SecondTestDestinationId,
                    DestinationName = SecondTestDestinationName,
                    Type = TestRestaurantType,
                },
            };

            var actual = await this.RestaurantsServiceMock.GetAllAsync();

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                    Assert.Equal(expected[0].DestinationName, elem1.DestinationName);
                    Assert.Equal(expected[0].Type, elem1.Type);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                    Assert.Equal(expected[1].DestinationName, elem2.DestinationName);
                    Assert.Equal(expected[1].Type, elem2.Type);
                });
            Assert.Equal(expected.Length, actual.Count());
        }

        [Fact]
        public async Task GetAllInDestinationAsyncReturnsAllRestaurantsInDestination()
        {
            await this.AddTestingDestinationToDb();
            this.DbContext.Destinations.Add(new Destination
            { Id = SecondTestDestinationId, Name = SecondTestDestinationName });
            await this.DbContext.SaveChangesAsync();

            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.Bar,
            });
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = SecondTestRestaurantId,
                Name = SecondTestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.Bar,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new RestaurantViewModel[]
            {
                new RestaurantViewModel
                {
                    Id = TestRestaurantId,
                    Name = TestRestaurantName,
                    DestinationId = TestDestinationId,
                    DestinationName = TestDestinationName,
                    Type = TestRestaurantType,
                },
                new RestaurantViewModel
                {
                    Id = SecondTestRestaurantId,
                    Name = SecondTestRestaurantName,
                    DestinationId = TestDestinationId,
                    DestinationName = TestDestinationName,
                    Type = TestRestaurantType,
                },
            };

            var actual = await this.RestaurantsServiceMock.GetAllInDestinationAsync(TestDestinationId);

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                    Assert.Equal(expected[0].DestinationName, elem1.DestinationName);
                    Assert.Equal(expected[0].Type, elem1.Type);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                    Assert.Equal(expected[1].DestinationName, elem2.DestinationName);
                    Assert.Equal(expected[1].Type, elem2.Type);
                });
            Assert.Equal(expected.Length, actual.Count());
        }

        [Fact]
        public async Task GetAllInDestinationDoesNotReturnRestaurantsInOtherDestinations()
        {
            await this.AddTestingDestinationToDb();
            this.DbContext.Destinations.Add(new Destination
            {
                Id = SecondTestDestinationId,
                Name = SecondTestDestinationName
            });
            await this.DbContext.SaveChangesAsync();

            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.Bar,
            });
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = SecondTestRestaurantId,
                Name = SecondTestRestaurantName,
                DestinationId = SecondTestDestinationId,
                Type = RestaurantType.Bar,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new RestaurantViewModel[]
            {
                new RestaurantViewModel
                {
                    Id = SecondTestRestaurantId,
                    Name = SecondTestRestaurantName,
                    DestinationId = SecondTestDestinationId,
                    DestinationName = SecondTestDestinationName,
                    Type = TestRestaurantType,
                },
            };

            var actual = await this.RestaurantsServiceMock.GetAllInDestinationAsync(SecondTestDestinationId);

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                    Assert.Equal(expected[0].DestinationName, elem1.DestinationName);
                    Assert.Equal(expected[0].Type, elem1.Type);
                });
            Assert.Equal(expected.Length, actual.Count());
        }

        [Fact]
        public async Task GetViewModelByIdAsyncThrowsNullReferenceExceptionIfRestaurantNotFound()
        {
            await this.AddTestingRestaurantToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                 this.RestaurantsServiceMock.GetViewModelByIdAsync<RestaurantDetailsViewModel>(SecondTestRestaurantId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceRestaurantId, SecondTestRestaurantId), exception.Message);
        }

        [Fact]
        public async Task GetViewModelByIdAsyncReturnsCorrectViewModel()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();

            var expected = this.DbContext.Restaurants.OrderBy(r => r.CreatedOn);
            var actual = await this.RestaurantsServiceMock.GetViewModelByIdAsync<RestaurantViewModel>(TestRestaurantId);

            Assert.IsType<RestaurantViewModel>(actual);
            Assert.Collection(expected,
                elem1 =>
                {
                    Assert.Equal(expected.First().Id, actual.Id);
                    Assert.Equal(expected.First().Name, actual.Name);
                    Assert.Equal(expected.First().DestinationId, actual.DestinationId);
                    Assert.Equal(expected.First().Destination.Name, actual.DestinationName);
                    Assert.Equal(expected.First().Type.ToString(), actual.Type);
                });
        }

        [Fact]
        public async Task DeleteByIdAsyncThrowsNullReferenceExceptionIfRestaurantNotFound()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.RestaurantsServiceMock.DeleteByIdAsync(SecondTestRestaurantId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceRestaurantId, SecondTestRestaurantId), exception.Message);
        }

        [Fact]
        public async Task DeleteByIdAsyncDeletesTheCorrectRestaurant()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();

            var restaurantToDelete = new Restaurant
            {
                Id = 3,
                Name = "Pri Ivan",
                DestinationId = TestDestinationId,
                Type = RestaurantType.FineDining,
                IsDeleted = false,
            };
            this.DbContext.Restaurants.Add(restaurantToDelete);
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = SecondTestRestaurantId,
                Name = SecondTestDestinationName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.FastFood,
                IsDeleted = false,
            });
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = 4,
                Name = "Pri Spas",
                DestinationId = TestDestinationId,
                Type = RestaurantType.Pub,
                IsDeleted = false,
            });
            await this.DbContext.SaveChangesAsync();

            await this.RestaurantsServiceMock.DeleteByIdAsync(3);
            Assert.DoesNotContain(restaurantToDelete, this.DbContext.Restaurants);
        }

        [Fact]
        public async Task DeleteByIdOnlyDeletesOneRestaurant()
        {
            await this.AddTestingDestinationToDb();

            var firstRestaurant = new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.Bar,
            };
            this.DbContext.Restaurants.Add(firstRestaurant);
            var secondRestaurant = new Restaurant
            {
                Id = SecondTestRestaurantId,
                Name = SecondTestDestinationName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.FastFood,
            };
            this.DbContext.Restaurants.Add(secondRestaurant);
            var restaurantToDelete = new Restaurant
            {
                Id = 3,
                Name = "Pri Ivan",
                DestinationId = TestDestinationId,
                Type = RestaurantType.FineDining,
            };
            this.DbContext.Restaurants.Add(restaurantToDelete);
            await this.DbContext.SaveChangesAsync();

            await this.RestaurantsServiceMock.DeleteByIdAsync(SecondTestRestaurantId);

            var expectedDbSetCount = 2;
            Assert.Equal(expectedDbSetCount, this.DbContext.Restaurants.Count());
        }

        [Fact]
        public async Task CreateAsyncThrowsArgumentExceptionIdRestaurantTypeInvalid()
        {
            await this.AddTestingDestinationToDb();
            var invalidRestaurantCreateInputModel = new RestaurantCreateInputModel
            {
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = InvalidRestaurantType,
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.RestaurantsServiceMock.CreateAsync(invalidRestaurantCreateInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.InvalidRestaurantType, invalidRestaurantCreateInputModel.Type), exception.Message);
        }

        [Fact]
        public async Task CreateAsyncAddsRestaurantToDbContext()
        {
            await this.AddTestingDestinationToDb();

            RestaurantDetailsViewModel restaurantDetailsViewModel;
            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var restaurantCreateInputModel = new RestaurantCreateInputModel
                {
                    Name = TestRestaurantName,
                    DestinationId = TestDestinationId,
                    Type = TestRestaurantType,
                    Image = file,
                    Address = TestRestaurantAddress,
                    Seats = TestRestaurantSeats,
                };

                restaurantDetailsViewModel = await this.RestaurantsServiceMock.CreateAsync(restaurantCreateInputModel);
            }

            ApplicationCloudinary.DeleteImage(ServiceProvider.GetRequiredService<Cloudinary>(), restaurantDetailsViewModel.Name);

            var restaurantsDbSet = this.DbContext.Restaurants.OrderBy(r => r.CreatedOn);

            Assert.Collection(restaurantsDbSet,
                elem1 =>
            {
                Assert.Equal(restaurantsDbSet.Last().Id, restaurantDetailsViewModel.Id);
                Assert.Equal(restaurantsDbSet.Last().Name, restaurantDetailsViewModel.Name);
                Assert.Equal(restaurantsDbSet.Last().DestinationId, restaurantDetailsViewModel.DestinationId);
                Assert.Equal(restaurantsDbSet.Last().Destination.Name, restaurantDetailsViewModel.DestinationName);
                Assert.Equal(restaurantsDbSet.Last().Address, restaurantDetailsViewModel.Address);
                Assert.Equal(restaurantsDbSet.Last().Type.ToString(), restaurantDetailsViewModel.Type);
                Assert.Equal(restaurantsDbSet.Last().ImageUrl, restaurantDetailsViewModel.ImageUrl);
            });
        }

        [Fact]
        public async Task CreateAsyncReturnsExistingViewModelIfRestaurantExists()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();

            RestaurantDetailsViewModel restaurantDetailsViewModel;
            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var restaurantCreateInputModel = new RestaurantCreateInputModel
                {
                    Name = TestRestaurantName,
                    DestinationId = TestDestinationId,
                    Type = TestRestaurantType,
                    Image = file,
                    Address = TestRestaurantAddress,
                    Seats = TestRestaurantSeats,
                };

                restaurantDetailsViewModel = await this.RestaurantsServiceMock.CreateAsync(restaurantCreateInputModel);
            }

            ApplicationCloudinary.DeleteImage(ServiceProvider.GetRequiredService<Cloudinary>(), restaurantDetailsViewModel.Name);

            var restaurantsDbSet = this.DbContext.Restaurants.OrderBy(r => r.CreatedOn);

            Assert.Equal(restaurantsDbSet.Last().Id, restaurantDetailsViewModel.Id);
            Assert.Equal(restaurantsDbSet.Last().Name, restaurantDetailsViewModel.Name);
            Assert.Equal(restaurantsDbSet.Last().DestinationId, restaurantDetailsViewModel.DestinationId);
            Assert.Equal(restaurantsDbSet.Last().Destination.Name, restaurantDetailsViewModel.DestinationName);
            Assert.Equal(restaurantsDbSet.Last().Address, restaurantDetailsViewModel.Address);
            Assert.Equal(restaurantsDbSet.Last().Type.ToString(), restaurantDetailsViewModel.Type);
            Assert.Equal(restaurantsDbSet.Last().ImageUrl, restaurantDetailsViewModel.ImageUrl);
        }

        [Fact]
        public async Task EditAsyncThrowsArgumentExceptionIfRestaurantTypeInvalid()
        {
            await this.AddTestingDestinationToDb();
            var invalidRestaurantEditInputModel = new RestaurantEditViewModel()
            {
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = InvalidRestaurantType,
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.RestaurantsServiceMock.EditAsync(invalidRestaurantEditInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.InvalidRestaurantType, invalidRestaurantEditInputModel.Type), exception.Message);
        }

        [Fact]
        public async Task EditAsyncThrowsNullReferenceExceptionIfRestaurantNotFound()
        {
            await this.AddTestingDestinationToDb();
            var invalidRestaurantEditViewModel = new RestaurantEditViewModel()
            {
                Id = SecondTestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = TestRestaurantType,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.RestaurantsServiceMock.EditAsync(invalidRestaurantEditViewModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceRestaurantId, SecondTestRestaurantId), exception.Message);
        }

        [Fact]
        public async Task EditAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            await this.AddTestingRestaurantToDb();
            var invalidRestaurantEditViewModel = new RestaurantEditViewModel()
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = TestRestaurantType,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.RestaurantsServiceMock.EditAsync(invalidRestaurantEditViewModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, TestDestinationId), exception.Message);
        }

        [Fact]
        public async Task EditAsyncEditsRestaurantWhenImageStaysTheSame()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();

            var newAddress = "New address";
            var newSeats = 48;

            Assert.NotEqual(newAddress, this.DbContext.Restaurants.Find(TestRestaurantId).Address);
            Assert.NotEqual(newSeats, this.DbContext.Restaurants.Find(TestRestaurantId).Seats);

            var restaurantEditViewModel = new RestaurantEditViewModel()
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = TestRestaurantType,
                Address = newAddress,
                Seats = newSeats,
                NewImage = null,
            };

            await this.RestaurantsServiceMock.EditAsync(restaurantEditViewModel);

            Assert.Equal(newAddress, this.DbContext.Restaurants.Find(TestRestaurantId).Address);
            Assert.Equal(newSeats, this.DbContext.Restaurants.Find(TestRestaurantId).Seats);
        }

        [Fact]
        public async Task EditAsyncEditsRestaurantsImage()
        {
            await this.AddTestingDestinationToDb();
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.Bar,
                Address = TestRestaurantAddress,
                Seats = TestRestaurantSeats,
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

                var restaurantEditViewModel = new RestaurantEditViewModel()
                {
                    Id = TestRestaurantId,
                    Name = TestRestaurantName,
                    DestinationId = TestDestinationId,
                    Type = TestRestaurantType,
                    Address = TestRestaurantAddress,
                    Seats = TestRestaurantSeats,
                    NewImage = file,
                };

                await this.RestaurantsServiceMock.EditAsync(restaurantEditViewModel);

                ApplicationCloudinary.DeleteImage(ServiceProvider.GetRequiredService<Cloudinary>(), restaurantEditViewModel.Name);
            }

            Assert.NotEqual(TestImageUrl, this.DbContext.Restaurants.Find(TestRestaurantId).ImageUrl);
        }

        [Fact]
        public async Task ReviewThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.RestaurantsServiceMock.Review(TestRestaurantId, InvalidUsername, null));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, InvalidUsername), exception.Message);
        }

        [Fact]
        public async Task ReviewThrowsNullReferenceExceptionIfRestaurantNotFound()
        {
            await this.AddTestingDestinationToDb();
            await AddTestingUserToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.RestaurantsServiceMock.Review(TestRestaurantId, TestUserName, null));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceRestaurantId, TestRestaurantId), exception.Message);
        }

        [Fact]
        public async Task ReviewAddsNewReviewToDbContext()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();
            await this.AddTestingUserToDb();

            var restaurantReviewInputModel = new RestaurantReviewInputModel
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };

            await this.RestaurantsServiceMock.Review(TestRestaurantId, TestUserName, restaurantReviewInputModel);

            var reviewsDbSet = this.DbContext.Reviews.OrderBy(r => r.CreatedOn);

            Assert.Collection(reviewsDbSet,
                elem1 =>
                {
                    Assert.Equal(reviewsDbSet.Last().Rating, restaurantReviewInputModel.Rating);
                    Assert.Equal(reviewsDbSet.Last().Content, restaurantReviewInputModel.Content);
                });
        }

        [Fact]
        public async Task ReviewAddsNewRestaurantReviewToDbContext()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();
            await this.AddTestingUserToDb();

            var restaurantReviewInputModel = new RestaurantReviewInputModel
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };

            await this.RestaurantsServiceMock.Review(TestRestaurantId, TestUserName, restaurantReviewInputModel);

            var reviewId = this.DbContext.Reviews.Last().Id;
            var reviewsDbSet = this.DbContext.RestaurantReviews.OrderBy(r => r.CreatedOn);

            Assert.Collection(reviewsDbSet,
                elem1 =>
                {
                    Assert.Equal(reviewsDbSet.Last().RestaurantId, restaurantReviewInputModel.Id);
                    Assert.Equal(reviewsDbSet.Last().ReviewId, reviewId);
                });
        }

        [Fact]
        public async Task UpdateRestaurantAverageRatingCalculatesRatingCorrectly()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();
            await this.AddTestingUserToDb();

            this.DbContext.Users.Add(new UnravelTravelUser { Id = Guid.NewGuid().ToString(), UserName = "Ivan" });
            await this.DbContext.SaveChangesAsync();

            var restaurantReviewInputModel = new RestaurantReviewInputModel
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };

            await this.RestaurantsServiceMock.Review(TestRestaurantId, TestUserName, restaurantReviewInputModel);

            var secondRestaurantReviewInputModel = new RestaurantReviewInputModel
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                Rating = SecondTestReviewRating,
                Content = TestReviewContent,
            };
            await this.RestaurantsServiceMock.Review(TestRestaurantId, "Ivan", secondRestaurantReviewInputModel);

            var expected = (TestReviewRating + SecondTestReviewRating) / 2;
            var actual = this.DbContext.Restaurants.Find(TestRestaurantId).AverageRating;

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ReviewThrowsArgumentExceptionIfUserHasAlreadyReviewedRestaurant()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();
            await this.AddTestingUserToDb();

            var restaurantReviewInputModel = new RestaurantReviewInputModel
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };

            await this.RestaurantsServiceMock.Review(TestRestaurantId, TestUserName, restaurantReviewInputModel);

            var secondRestaurantReviewInputModel = new RestaurantReviewInputModel
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                Rating = SecondTestReviewRating,
                Content = TestReviewContent,
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.RestaurantsServiceMock.Review(TestRestaurantId, TestUserName, secondRestaurantReviewInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.RestaurantReviewAlreadyAdded, TestUserName, TestRestaurantId, TestRestaurantName), exception.Message);
        }

        [Theory]
        [InlineData(TestRestaurantName, 1, null)]
        [InlineData(TestRestaurantAddress, 1, null)]
        [InlineData(TestUserName, 0, null)]
        [InlineData(TestRestaurantName, 1, 1)]
        [InlineData(TestRestaurantName, 0, 2)]
        public async Task GetRestaurantsFromSearchReturnsAllRestaurantsContainingSearchString(string searchString, int expectedCount, int? destinationId)
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingRestaurantToDb();
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = SecondTestRestaurantId,
                Name = SecondTestRestaurantName,
                DestinationId = SecondTestDestinationId,
                Type = RestaurantType.CoffeeShop,
                Address = SecondTestRestaurantAddress,
                Seats = TestRestaurantSeats,
            });
            await this.DbContext.SaveChangesAsync();

            var actual = this.RestaurantsServiceMock.GetRestaurantsFromSearch(searchString, destinationId);

            Assert.Equal(expectedCount, actual.Count());
        }

        [Theory]
        [InlineData(RestaurantSorter.Name, "Aaa")]
        [InlineData(null, "Aaa")]
        [InlineData(RestaurantSorter.Rating, "Zzz")]
        [InlineData(RestaurantSorter.Type, TestRestaurantName)]
        [InlineData(RestaurantSorter.Destination, SecondTestRestaurantName)]
        public async Task SortBySortsRestaurantAsExpected(RestaurantSorter sorter, string expectedFirstRestaurantName)
        {
            await this.AddTestingDestinationToDb();
            this.DbContext.Restaurants.AddRange(new List<Restaurant>
            {
                new Restaurant
                {
                    Name = TestRestaurantName,
                    DestinationId = TestDestinationId,
                    Type = RestaurantType.Bar,
                    AverageRating = 0,
                },
                new Restaurant
                {
                    Name = SecondTestRestaurantName,
                    DestinationId = SecondTestDestinationId,
                    Type = RestaurantType.CoffeeShop,
                    AverageRating = 1,
                },
                new Restaurant
                {
                    Name = "Aaaaa",
                    DestinationId = SecondTestDestinationId,
                    Type = RestaurantType.FineDining,
                    AverageRating = 2,
                },
                new Restaurant
                {
                    Name = "Zzzzz",
                    DestinationId = SecondTestDestinationId,
                    Type = RestaurantType.FineDining,
                    AverageRating = 5,
                },
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new RestaurantViewModel[]
            {
                new RestaurantViewModel
                {
                    Name = TestRestaurantName,
                    DestinationId = SecondTestDestinationId,
                    DestinationName = "Zzz",
                    Type = RestaurantType.Bar.ToString(),
                    AverageRating = 0,
                },
                new RestaurantViewModel
                {
                    Name = SecondTestRestaurantName,
                    DestinationId = TestDestinationId,
                    DestinationName = "Aaa",
                    Type = RestaurantType.CoffeeShop.ToString(),
                    AverageRating = 1,
                },
                new RestaurantViewModel
                {
                    Name = "Aaa",
                    DestinationId = SecondTestDestinationId,
                    DestinationName = "Aaa",
                    Type = RestaurantType.FineDining.ToString(),
                    AverageRating = 2,
                },
                new RestaurantViewModel
                {
                    Name = "Zzz",
                    DestinationId = SecondTestDestinationId,
                    DestinationName = "Aaa",
                    Type = RestaurantType.FineDining.ToString(),
                    AverageRating = 5,
                },
            };

            var actual = this.RestaurantsServiceMock.SortBy(expected, sorter);
            Assert.Equal(expectedFirstRestaurantName, actual.First().Name);
        }

        private async Task AddTestingUserToDb()
        {
            this.DbContext.Add(new UnravelTravelUser { Id = this.testUserId, UserName = TestUserName });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingRestaurantToDb()
        {
            this.DbContext.Restaurants.Add(new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
                DestinationId = TestDestinationId,
                Type = RestaurantType.Bar,
                Address = TestRestaurantAddress,
                Seats = TestRestaurantSeats,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingDestinationToDb()
        {
            this.DbContext.Destinations.Add(new Destination
            {
                Id = TestDestinationId,
                Name = TestDestinationName,
            });
            await this.DbContext.SaveChangesAsync();
        }
    }
}

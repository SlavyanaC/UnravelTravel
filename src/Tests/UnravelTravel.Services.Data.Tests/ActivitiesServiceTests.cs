using System.Collections.Generic;
using UnravelTravel.Models.ViewModels.Enums;

namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using CloudinaryDotNet;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Activities;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using Xunit;

    public class ActivitiesServiceTests : BaseServiceTests
    {
        private const int TestCountryId = 1;
        private const string TestCountryName = "Bulgaria";

        private const int TestDestinationId = 1;
        private const string TestDestinationName = "Sofia";
        private const int SecondTestDestinationId = 2;
        private const string SecondTestDestinationName = "London";

        private const int TestActivityId = 1;
        private const string TestActivityName = "Test Activity 123";
        private const string TestActivityType = "Adventure";
        private const string TestActivityAddress = "bul. Bulgaria 102";
        private const string SecondTestActivityAddress = "15 Vitosha Str.";

        private const int SecondTestActivityId = 2;
        private const string SecondTestActivityName = "Secondd Activity";

        private const string InvalidActivityType = "InvalidType";

        private const string TestImageUrl = "https://someurl.com";
        private const string TestImagePath = "Test.jpg";
        private const string TestImageContentType = "image/jpg";

        private const string TestUserName = "Pesho";
        private const string InvalidUsername = "Stamat";
        private const double TestReviewRating = 4.7;

        private const double SecondTestReviewRating = 1.2;
        private const string TestReviewContent = "Testing review.";

        private readonly string testUserId = Guid.NewGuid().ToString();
        private readonly DateTime testDate = DateTime.Now.AddDays(2);

        private IActivitiesService ActivitiesServiceMock => this.ServiceProvider.GetRequiredService<IActivitiesService>();

        [Fact]
        public async Task GetAllAsyncReturnsAllActivities()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            this.DbContext.Activities.Add(new Activity
            {
                Id = TestActivityId,
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Type = ActivityType.Adventure,
            });
            this.DbContext.Activities.Add(new Activity
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                DestinationId = TestDestinationId,
                Type = ActivityType.Adventure,
                Date = testDate,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new ActivityViewModel[]
            {
                new ActivityViewModel
                {
                    Id = TestActivityId,
                    Name = TestActivityName,
                    DestinationId = TestDestinationId,
                    DestinationName = TestDestinationName,
                    Type = TestActivityType,
                    Date = testDate,
                },
                new ActivityViewModel
                {
                    Id = SecondTestActivityId,
                    Name = SecondTestActivityName,
                    DestinationId = TestDestinationId,
                    DestinationName = TestDestinationName,
                    Type = TestActivityType,
                    Date = testDate,
                },
            };

            var actual = await this.ActivitiesServiceMock.GetAllAsync();

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
        public async Task GetAllInDestinationAsyncReturnsAllActivitiesInDestination()
        {
            await this.AddTestingCountryToDb();
            this.DbContext.Destinations.Add(new Destination
            {
                Id = SecondTestDestinationId,
                Name = SecondTestDestinationName,
                CountryId = TestCountryId,
            });
            this.DbContext.Activities.AddRange(new List<Activity>
            {
                new Activity
                {
                    Id = TestActivityId,
                    Name = TestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Adventure,
                },
                new Activity
                {
                    Id = SecondTestActivityId,
                    Name = SecondTestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Other,
                }
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new ActivityViewModel[]
            {
                new ActivityViewModel
                {
                    Id = TestActivityId,
                    Name = TestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Adventure.ToString(),
                },
                new ActivityViewModel
                {
                    Id = SecondTestActivityId,
                    Name = SecondTestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Other.ToString(),
                },
            };

            var actual = await this.ActivitiesServiceMock.GetAllInDestinationAsync(SecondTestDestinationId);

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                    Assert.Equal(expected[0].DestinationId, elem1.DestinationId);
                    Assert.Equal(expected[0].Type, elem1.Type);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                    Assert.Equal(expected[1].DestinationId, elem2.DestinationId);
                    Assert.Equal(expected[1].Type, elem2.Type);
                });
            Assert.Equal(expected.Length, actual.Count());
        }

        [Fact]
        public async Task GetAllInDestinationDoesNotReturnActivitiesInOtherDestinations()
        {
            await this.AddTestingCountryToDb();
            this.DbContext.Destinations.Add(new Destination
            {
                Id = SecondTestDestinationId,
                Name = SecondTestDestinationName,
                CountryId = TestCountryId,
            });
            this.DbContext.Activities.AddRange(new List<Activity>
            {
                new Activity
                {
                    Id = TestActivityId,
                    Name = TestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Adventure,
                },
                new Activity
                {
                    Id = SecondTestActivityId,
                    Name = SecondTestActivityName,
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Other,
                }
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new ActivityViewModel[]
            {
                new ActivityViewModel
                {
                    Id = TestActivityId,
                    Name = TestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Adventure.ToString(),
                },
            };

            var actual = await this.ActivitiesServiceMock.GetAllInDestinationAsync(SecondTestDestinationId);

            Assert.Equal(expected[0].Id, actual.First().Id);
            Assert.Equal(expected[0].Name, actual.First().Name);
            Assert.Equal(expected[0].DestinationId, actual.First().DestinationId);
            Assert.Equal(expected[0].Type, actual.First().Type);

            Assert.Equal(expected.Length, actual.Count());
        }

        [Fact]
        public async Task GetViewModelByIdAsyncThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingActivityToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ActivitiesServiceMock.GetViewModelByIdAsync<RestaurantDetailsViewModel>(SecondTestActivityId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, SecondTestActivityId), exception.Message);
        }

        [Fact]
        public async Task GetViewModelByIdAsyncReturnsCorrectViewModel()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();

            var expected = this.DbContext.Activities.OrderBy(r => r.CreatedOn);
            var actual = await this.ActivitiesServiceMock.GetViewModelByIdAsync<ActivityViewModel>(TestActivityId);

            Assert.IsType<ActivityViewModel>(actual);
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
        public async Task DeleteByIdAsyncThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ActivitiesServiceMock.DeleteByIdAsync(SecondTestActivityId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, SecondTestActivityId), exception.Message);
        }

        [Fact]
        public async Task DeleteByIdAsyncDeletesTheCorrectActivity()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();

            var activityToDelete = new Activity()
            {
                Id = 3,
                Name = "To delete",
                DestinationId = TestDestinationId,
                Type = ActivityType.Adventure,
                IsDeleted = false,
            };
            this.DbContext.Activities.Add(activityToDelete);
            this.DbContext.Activities.Add(new Activity()
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                DestinationId = TestDestinationId,
                Type = ActivityType.Adventure,
                IsDeleted = false,
            });
            this.DbContext.Activities.Add(new Activity
            {
                Id = 4,
                Name = "Another",
                DestinationId = TestDestinationId,
                Type = ActivityType.Adventure,
                IsDeleted = false,
            });
            await this.DbContext.SaveChangesAsync();

            await this.ActivitiesServiceMock.DeleteByIdAsync(activityToDelete.Id);
            Assert.DoesNotContain(activityToDelete, this.DbContext.Activities);
        }

        [Fact]
        public async Task DeleteByIdOnlyDeletesOneActivity()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();

            var secondActivity = new Activity()
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                DestinationId = TestDestinationId,
                Type = ActivityType.Extreme,
            };
            var activityToDelete = new Activity
            {
                Id = 3,
                Name = "To be deleted",
                DestinationId = TestDestinationId,
                Type = ActivityType.Culinary,
            };
            this.DbContext.Activities.Add(secondActivity);
            this.DbContext.Activities.Add(activityToDelete);
            await this.DbContext.SaveChangesAsync();

            await this.ActivitiesServiceMock.DeleteByIdAsync(activityToDelete.Id);

            var expectedDbSetCount = 2;
            Assert.Equal(expectedDbSetCount, this.DbContext.Activities.Count());
        }

        [Fact]
        public async Task CreateAsyncThrowsArgumentExceptionIdActivityTypeInvalid()
        {
            await this.AddTestingDestinationToDb();

            var invalidActivityCreateInputModel = new ActivityCreateInputModel
            {
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Type = InvalidActivityType,
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.ActivitiesServiceMock.CreateAsync(invalidActivityCreateInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.InvalidActivityType, invalidActivityCreateInputModel.Type), exception.Message);
        }

        [Fact]
        public async Task CreateAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            var invalidActivityCreateInputModel = new ActivityCreateInputModel
            {
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Type = TestActivityType,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ActivitiesServiceMock.CreateAsync(invalidActivityCreateInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, invalidActivityCreateInputModel.DestinationId), exception.Message);
        }

        [Fact]
        public async Task CreateAsyncAddsActivityToDbContext()
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();

            ActivityDetailsViewModel activityDetailsViewModel;
            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var activityCreateInputModel = new ActivityCreateInputModel()
                {
                    Name = TestActivityName,
                    DestinationId = TestDestinationId,
                    Type = TestActivityType,
                    Image = file,
                    Date = this.testDate,
                };

                activityDetailsViewModel = await this.ActivitiesServiceMock.CreateAsync(activityCreateInputModel);
            }

            ApplicationCloudinary.DeleteImage(ServiceProvider.GetRequiredService<Cloudinary>(), activityDetailsViewModel.Name);

            var activitiesDbSet = this.DbContext.Activities.OrderBy(r => r.CreatedOn);

            Assert.Collection(activitiesDbSet,
                elem1 =>
                {
                    Assert.Equal(activitiesDbSet.Last().Id, activityDetailsViewModel.Id);
                    Assert.Equal(activitiesDbSet.Last().Name, activityDetailsViewModel.Name);
                    Assert.Equal(activitiesDbSet.Last().DestinationId, activityDetailsViewModel.DestinationId);
                    Assert.Equal(activitiesDbSet.Last().Type.ToString(), activityDetailsViewModel.Type);
                    Assert.Equal(activitiesDbSet.Last().ImageUrl, activityDetailsViewModel.ImageUrl);
                });
        }

        [Fact]
        public async Task EditAsyncThrowsArgumentExceptionIfActivityTypeInvalid()
        {
            await this.AddTestingDestinationToDb();

            var invalidActivityEditInputModel = new ActivityEditViewModel()
            {
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Type = InvalidActivityType,
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.ActivitiesServiceMock.EditAsync(invalidActivityEditInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.InvalidActivityType, invalidActivityEditInputModel.Type), exception.Message);
        }

        [Fact]
        public async Task EditAsyncThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingDestinationToDb();

            var invalidActivityEditViewModel = new ActivityEditViewModel()
            {
                Id = SecondTestActivityId,
                Name = SecondTestActivityName,
                DestinationId = TestDestinationId,
                Type = TestActivityType,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ActivitiesServiceMock.EditAsync(invalidActivityEditViewModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, invalidActivityEditViewModel.Id), exception.Message);
        }

        [Fact]
        public async Task EditAsyncThrowsNullReferenceExceptionIfDestinationNotFound()
        {
            await this.AddTestingActivityToDb();
            var invalidActivityEditViewModel = new ActivityEditViewModel()
            {
                Id = TestActivityId,
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Type = TestActivityType,
            };

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ActivitiesServiceMock.EditAsync(invalidActivityEditViewModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceDestinationId, invalidActivityEditViewModel.DestinationId), exception.Message);
        }

        [Fact]
        public async Task EditAsyncEditsRestaurantWhenImageStaysTheSame()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();

            await this.DbContext.SaveChangesAsync();

            this.DbContext.Destinations.Add(new Destination { Id = SecondTestDestinationId, Name = SecondTestDestinationName });
            await this.DbContext.SaveChangesAsync();

            var newName = SecondTestActivityName;
            var newDestinationId = 2;

            Assert.NotEqual(newName, this.DbContext.Activities.Find(TestActivityId).Name);

            var activityEditViewModel = new ActivityEditViewModel()
            {
                Id = TestActivityId,
                Name = newName,
                DestinationId = newDestinationId,
                Type = TestActivityType,
                NewImage = null,
            };

            await this.ActivitiesServiceMock.EditAsync(activityEditViewModel);
            Assert.Equal(newName, this.DbContext.Activities.Find(TestActivityId).Name);
        }

        [Fact]
        public async Task EditAsyncEditsRestaurantsImage()
        {
            await this.AddTestingDestinationToDb();

            this.DbContext.Activities.Add(new Activity
            {
                Id = TestActivityId,
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Type = ActivityType.Adventure,
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

                var activityToEditViewModel = new ActivityEditViewModel()
                {
                    Id = TestActivityId,
                    Name = TestActivityName,
                    DestinationId = TestDestinationId,
                    Type = TestActivityType,
                    NewImage = file,
                };

                await this.ActivitiesServiceMock.EditAsync(activityToEditViewModel);

                ApplicationCloudinary.DeleteImage(ServiceProvider.GetRequiredService<Cloudinary>(), activityToEditViewModel.Name);
            }

            Assert.NotEqual(TestImageUrl, this.DbContext.Activities.Find(TestActivityId).ImageUrl);
        }

        [Fact]
        public async Task ReviewThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.ActivitiesServiceMock.Review(TestActivityId, InvalidUsername, null));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, InvalidUsername), exception.Message);
        }

        [Fact]
        public async Task ReviewThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingUserToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.ActivitiesServiceMock.Review(TestActivityId, TestUserName, null));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, TestActivityId), exception.Message);
        }

        [Fact]
        public async Task ReviewAddsNewReviewToDbContext()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            var activityReviewInputModel = new ActivityReviewInputModel
            {
                Id = TestActivityId,
                Name = TestActivityName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };

            await this.ActivitiesServiceMock.Review(TestActivityId, TestUserName, activityReviewInputModel);
            var reviewsDbSet = this.DbContext.Reviews.OrderBy(r => r.CreatedOn);

            Assert.Collection(reviewsDbSet,
                elem1 =>
                {
                    Assert.Equal(reviewsDbSet.Last().Rating, activityReviewInputModel.Rating);
                    Assert.Equal(reviewsDbSet.Last().Content, activityReviewInputModel.Content);
                });
        }

        [Fact]
        public async Task ReviewAddsNewActivityReviewToDbContext()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            var activityReviewInputModel = new ActivityReviewInputModel()
            {
                Id = TestActivityId,
                Name = TestActivityName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };

            await this.ActivitiesServiceMock.Review(TestActivityId, TestUserName, activityReviewInputModel);

            var reviewId = this.DbContext.Reviews.Last().Id;
            var reviewsDbSet = this.DbContext.ActivityReviews.OrderBy(r => r.CreatedOn);

            Assert.Collection(reviewsDbSet,
                elem1 =>
                {
                    Assert.Equal(reviewsDbSet.Last().ActivityId, activityReviewInputModel.Id);
                    Assert.Equal(reviewsDbSet.Last().ReviewId, reviewId);
                });
        }

        [Fact]
        public async Task ReviewThrowsArgumentExceptionIfUserHasAlreadyReviewedRestaurant()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            var activityReviewInputModel = new ActivityReviewInputModel()
            {
                Id = TestActivityId,
                Name = TestActivityName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };

            await this.ActivitiesServiceMock.Review(TestActivityId, TestUserName, activityReviewInputModel);

            var secondActivityReviewInputModel = new ActivityReviewInputModel
            {
                Id = TestActivityId,
                Name = TestActivityName,
                Rating = SecondTestReviewRating,
                Content = TestReviewContent,
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.ActivitiesServiceMock.Review(TestActivityId, TestUserName, secondActivityReviewInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.ActivityReviewAlreadyAdded, TestUserName, TestActivityId, TestActivityName), exception.Message);
        }

        [Fact]
        public async Task UpdateRestaurantAverageRatingCalculatesRatingCorrectly()
        {
            await this.AddTestingDestinationToDb();
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            this.DbContext.Users.Add(new UnravelTravelUser { Id = Guid.NewGuid().ToString(), UserName = "Ivan" });
            await this.DbContext.SaveChangesAsync();

            var activityReviewInputModel = new ActivityReviewInputModel()
            {
                Id = TestActivityId,
                Name = TestActivityName,
                Rating = TestReviewRating,
                Content = TestReviewContent,
            };
            await this.ActivitiesServiceMock.Review(TestActivityId, TestUserName, activityReviewInputModel);

            var secondActivityReviewInputModel = new ActivityReviewInputModel()
            {
                Id = TestActivityId,
                Name = TestActivityName,
                Rating = SecondTestReviewRating,
                Content = TestReviewContent,
            };
            await this.ActivitiesServiceMock.Review(TestActivityId, "Ivan", secondActivityReviewInputModel);

            var expected = (TestReviewRating + SecondTestReviewRating) / 2;
            var actual = this.DbContext.Activities.Find(TestActivityId).AverageRating;

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(TestActivityName, 1, null)]
        [InlineData(SecondTestActivityAddress, 2, null)]
        [InlineData(TestUserName, 0, null)]
        [InlineData(TestActivityName, 1, TestDestinationId)]
        [InlineData(SecondTestActivityAddress, 1, SecondTestDestinationId)]
        public async Task GetActivitiesFromSearchReturnsAllActivitiesContainingSearchString(string searchString, int expectedCount, int? destinationId)
        {
            await this.AddTestingCountryToDb();
            await this.AddTestingDestinationToDb();
            this.DbContext.Destinations.Add(new Destination { Id = SecondTestDestinationId, Name = SecondTestDestinationName });
            this.DbContext.Activities.AddRange(new List<Activity>
            {
                new Activity
                {
                    Id=TestActivityId,
                    Name = TestActivityName,
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Shopping,
                    Address = SecondTestActivityAddress,
                    Description = "Some description here",

                },
                new Activity
                {
                    Id = SecondTestActivityId,
                    Name = SecondTestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Adventure,
                    Address = SecondTestActivityAddress,
                    Description = "Another description here",
                }
            });
            await this.DbContext.SaveChangesAsync();

            var actual = this.ActivitiesServiceMock.GetActivitiesFromSearch(searchString, destinationId);
            Assert.Equal(expectedCount, actual.Count());
        }

        [Theory]
        [InlineData(ActivitiesSorter.Upcoming, "Aaa")]
        [InlineData(null, "Aaa")]
        [InlineData(ActivitiesSorter.Name, "Aaa")]
        [InlineData(ActivitiesSorter.Destination, "Aaa")]
        [InlineData(ActivitiesSorter.Type, "Zzz")]
        public async Task SortBySortsRestaurantAsExpected(ActivitiesSorter sorter, string expectedFirstActivityName)
        {
            await this.AddTestingCountryToDb();
            this.DbContext.Destinations.AddRange(new List<Destination>
            {
                new Destination{Id = TestDestinationId, Name = TestDestinationName, CountryId = TestCountryId},
                new Destination{Id = SecondTestDestinationId, Name = "Aaa", CountryId = TestCountryId},
            });
            this.DbContext.Activities.AddRange(new List<Activity>
            {
                new Activity
                {
                    Name = "Aaa",
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Recreation,
                    Date = DateTime.Now.AddDays(1),
                },
                new Activity
                {
                    Name = TestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Culture,
                    Date = DateTime.Now.AddDays(2),
                }, // Destination
                new Activity
                {
                    Name = SecondTestActivityName,
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Shopping,
                    Date = DateTime.Now.AddDays(3),
                },
                new Activity
                {
                    Name = "Zzz",
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Adventure,
                    Date = DateTime.Now.AddDays(4),
                }, // Type
            });
            await this.DbContext.SaveChangesAsync();

            var activitiesToSort = new ActivityViewModel[]
            {
                new ActivityViewModel
                {
                    Name = "Aaa",
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Recreation.ToString(),
                    Date = DateTime.Now.AddDays(1),
                },
                new ActivityViewModel
                {
                    Name = TestActivityName,
                    DestinationId = SecondTestDestinationId,
                    Type = ActivityType.Culture.ToString(),
                    Date = DateTime.Now.AddDays(2),
                },
                new ActivityViewModel
                {
                    Name = SecondTestActivityName,
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Shopping.ToString(),
                    Date = DateTime.Now.AddDays(3),
                },
                new ActivityViewModel
                {
                    Name = "Zzz",
                    DestinationId = TestDestinationId,
                    Type = ActivityType.Adventure.ToString(),
                    Date = DateTime.Now.AddDays(4),
                },
            };

            var actual = this.ActivitiesServiceMock.SortBy(activitiesToSort, sorter);
            Assert.Equal(expectedFirstActivityName, actual.First().Name);
        }


        private async Task AddTestingUserToDb()
        {
            this.DbContext.Add(new UnravelTravelUser { Id = this.testUserId, UserName = TestUserName });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingActivityToDb()
        {
            this.DbContext.Activities.Add(new Activity
            {
                Id = TestActivityId,
                Name = TestActivityName,
                DestinationId = TestDestinationId,
                Type = ActivityType.Adventure,
                Date = this.testDate,
                Address = TestActivityAddress,
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
            this.DbContext.Countries.Add(new Country { Id = TestCountryId, Name = TestCountryName });
            await this.DbContext.SaveChangesAsync();
        }
    }
}

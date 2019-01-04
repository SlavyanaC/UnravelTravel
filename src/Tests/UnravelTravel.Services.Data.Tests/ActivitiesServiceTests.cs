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
        private const int TestDestinationId = 1;
        private const string TestDestinationName = "Bulgaria";
        private const int SecondTestDestinationId = 2;
        private const string SecondTestDestinationName = "USA";

        private const int TestActivityId = 1;
        private const string TestActivityName = "Test Activity 123";
        private const string TestActivityType = "Adventure";

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

        private IActivitiesService ActivitiesServiceMock => this.ServiceProvider.GetRequiredService<IActivitiesService>();

        [Fact]
        public async Task GetAllAsyncReturnsAllRestaurants()
        {
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
                },
                new ActivityViewModel
                {
                    Id = SecondTestActivityId,
                    Name = SecondTestActivityName,
                    DestinationId = TestDestinationId,
                    DestinationName = TestDestinationName,
                    Type = TestActivityType,
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
            Assert.Equal(expected.Length, actual.Length);
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

            this.DbContext.Destinations.Add(new Destination{Id = SecondTestDestinationId, Name = SecondTestDestinationName});
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

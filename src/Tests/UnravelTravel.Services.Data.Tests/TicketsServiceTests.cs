namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Models.ViewModels.Tickets;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using Xunit;

    public class TicketsServiceTests : BaseServiceTests
    {
        private const string TestUsername = "Pesho";
        private const string NotExistingUsername = "Stamat";
        private const string TestUserEmail = "pesho@pesho.pesho";

        private const int TestingActivityId = 1;
        private const string TestingActivityName = "Best Activity";

        private const int TestingTicketId = 1;

        private const int TestingDestinationId = 1;
        private const string TestingDestinationName = "Test Destination 123";

        private readonly string testingUserId = Guid.NewGuid().ToString();

        private ITicketsService TicketsServiceMock => this.ServiceProvider.GetRequiredService<ITicketsService>();

        [Fact]
        public async Task BookAllAsyncAddsTicketsToUserTickets()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            this.DbContext.Activities.Add(new Activity
            {
                Id = 2,
                Name = TestingActivityName,
                DestinationId = TestingDestinationId,
            });
            await this.DbContext.SaveChangesAsync();

            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();
            await this.TicketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels);

            var expectedTicketsCount = 2;
            var actualTicketsCount = this.DbContext.Tickets.Count();

            Assert.Equal(expectedTicketsCount, actualTicketsCount);
        }

        [Fact]
        public async Task BookAllAsyncClearsUsersShoppingCartWhenDone()
        {
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            this.DbContext.Activities.Add(new Activity
            {
                Id = 2,
                Name = TestingActivityName,
                DestinationId = TestingDestinationId,
            });
            await this.DbContext.SaveChangesAsync();

            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();
            await this.TicketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels);

            var expectedUsersShoppingCartItemsCount = 0;
            var actualUsersShoppingCartItemsCount = this.DbContext.ShoppingCartActivities.Where(sca => sca.ShoppingCart.UserId == TestUsername)?.Count();

            Assert.Equal(expectedUsersShoppingCartItemsCount, actualUsersShoppingCartItemsCount);
        }

        [Fact]
        public async Task BookAllAsyncThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.TicketsServiceMock.BookAllAsync(NotExistingUsername, shoppingCartActivityViewModels));

            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, NotExistingUsername), exception.Message);
        }

        [Fact]
        public async Task BookAllAsyncThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingUserToDb();
            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.TicketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, TestingActivityId), exception.Message);
        }

        [Fact]
        public async Task BookAllAsyncThrowsInvalidOperationExceptionIfZeroOrNegativeQuantity()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            var shoppingCartActivityViewModels = new ShoppingCartActivityViewModel[]
            {
                new ShoppingCartActivityViewModel
                {
                    Id = 1,
                    ActivityId = TestingActivityId,
                    Quantity = 1,
                    ShoppingCartUserId = testingUserId,
                },
                new ShoppingCartActivityViewModel
                {
                    Id = 2,
                    ActivityId = TestingActivityId,
                    Quantity = -1,
                    ShoppingCartUserId = testingUserId,
                },
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                this.TicketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels));
            Assert.Equal(ServicesDataConstants.ZeroOrNegativeQuantity, exception.Message);
        }

        [Fact]
        public async Task GetDetailsAsyncReturnsCorrectTicketDetails()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            this.DbContext.Tickets.Add(new Ticket
            {
                Id = TestingTicketId,
                ActivityId = TestingActivityId,
                UserId = testingUserId,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = this.DbContext.Tickets.OrderBy(t => t.CreatedOn);
            var actual = await this.TicketsServiceMock.GetDetailsAsync(TestingTicketId);

            Assert.Collection(expected,
                elem1 =>
                {
                    Assert.Equal(expected.First().Id, actual.Id);
                    Assert.Equal(expected.First().ActivityId, actual.ActivityId);
                    Assert.Equal(expected.First().UserId, actual.UserId);
                });
        }

        [Fact]
        public async Task GetDetailsAsyncThrowsNullReferenceExceptionIfTicketDoesNotExist()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.TicketsServiceMock.GetDetailsAsync(TestingTicketId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceTicketId, TestingTicketId), exception.Message);
        }

        [Fact]
        public async Task GetAllAsyncReturnsAllTicketsForTheUser()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            this.DbContext.Tickets.Add(new Ticket
            {
                Id = 1,
                UserId = testingUserId,
                ActivityId = TestingActivityId,
            });
            this.DbContext.Tickets.Add(new Ticket
            {
                Id = 2,
                UserId = testingUserId,
                ActivityId = TestingActivityId,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new TicketDetailsViewModel[]
            {
                new TicketDetailsViewModel
                {
                    Id = 1,
                    UserId = testingUserId,
                    ActivityId = TestingActivityId,
                },
                new TicketDetailsViewModel()
                {
                    Id = 2,
                    UserId = testingUserId,
                    ActivityId = TestingActivityId,
                }
            };

            var actual = await this.TicketsServiceMock.GetAllAsync(TestUsername);
            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].UserId, elem1.UserId);
                    Assert.Equal(expected[0].ActivityId, elem1.ActivityId);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].UserId, elem2.UserId);
                    Assert.Equal(expected[1].ActivityId, elem2.ActivityId);
                });
            Assert.Equal(expected.Length, actual.Length);
        }

        [Fact]
        public async Task GetAllAsyncDoesNotReturnAnotherUsersTickets()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            this.DbContext.Tickets.Add(new Ticket
            {
                Id = 1,
                UserId = testingUserId,
                ActivityId = TestingActivityId,
            });
            this.DbContext.Tickets.Add(new Ticket
            {
                Id = 2,
                UserId = Guid.NewGuid().ToString(),
                ActivityId = TestingActivityId,
            });
            this.DbContext.Tickets.Add(new Ticket
            {
                Id = 3,
                UserId = Guid.NewGuid().ToString(),
                ActivityId = TestingActivityId,
            });
            await this.DbContext.SaveChangesAsync();

            var expected = new TicketDetailsViewModel[]
            {
                new TicketDetailsViewModel
                {
                    Id = 1,
                    UserId = testingUserId,
                    ActivityId = TestingActivityId,
                },
            };

            var actual = await this.TicketsServiceMock.GetAllAsync(TestUsername);
            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].UserId, elem1.UserId);
                    Assert.Equal(expected[0].ActivityId, elem1.ActivityId);
                });
            Assert.Equal(expected.Length, actual.Length);
        }

        private async Task AddTestingUserToDb()
        {
            this.DbContext.Users.Add(new UnravelTravelUser
            {
                Id = testingUserId,
                UserName = TestUsername,
                Email = TestUserEmail,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingActivityToDb()
        {
            await this.AddTestingDestinationToDb();
            this.DbContext.Activities.Add(new Activity
            {
                Id = TestingActivityId,
                Name = TestingActivityName,
                DestinationId = TestingDestinationId,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private async Task AddTestingDestinationToDb()
        {
            this.DbContext.Destinations.Add(new Destination()
            {
                Id = TestingDestinationId,
                Name = TestingDestinationName,
            });
            await this.DbContext.SaveChangesAsync();
        }

        private ShoppingCartActivityViewModel[] GetShoppingCartActivityViewModels()
        {
            return new ShoppingCartActivityViewModel[]
            {
                new ShoppingCartActivityViewModel
                {
                    Id = 1,
                    ActivityId = TestingActivityId,
                    Quantity = 1,
                    ShoppingCartUserId = testingUserId,
                },
                new ShoppingCartActivityViewModel
                {
                    Id = 2,
                    ActivityId = 2,
                    Quantity = 2,
                    ShoppingCartUserId = testingUserId,
                },
            };
        }
    }
}

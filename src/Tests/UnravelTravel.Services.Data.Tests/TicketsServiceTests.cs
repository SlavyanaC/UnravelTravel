namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.InputModels.Account;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Models.ViewModels.Tickets;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;
    using Xunit;

    public class TicketsServiceTests : BaseServiceTests
    {
        private const string TestUsername = "Pesho";
        private const string NotExistingUsername = "Stamat";
        private const int TestingActivityId = 1;
        private const string TestingActivityName = "Best Activity";
        private const int TestingTicketId = 1;
        private const int TestingLocationId = 1;
        private const string TestingLocationName = "Arena Armeec";

        private readonly string testingUserId = Guid.NewGuid().ToString();
        private readonly ITicketsService ticketsServiceMock;

        public TicketsServiceTests()
        {
            // Don't know why this sometimes works sometimes not
            Mapper.Reset();
            AutoMapperConfig.RegisterMappings(typeof(LoginInputModel).GetTypeInfo().Assembly);
            this.ticketsServiceMock = this.Provider.GetRequiredService<ITicketsService>();
        }

        [Fact]
        public async Task BookAllAsyncAddsTicketsToUserTickets()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            this.Context.Activities.Add(new Activity
            {
                Id = 2,
                Name = TestingActivityName,
                LocationId = TestingLocationId,
            });
            await this.Context.SaveChangesAsync();

            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();
            await this.ticketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels);

            var expectedTicketsCount = 2;
            var actualTicketsCount = this.Context.Tickets.Count();

            Assert.Equal(expectedTicketsCount, actualTicketsCount);
        }

        [Fact]
        public async Task BookAllAsyncClearsUsersShoppingCartWhenDone()
        {
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            this.Context.Activities.Add(new Activity
            {
                Id = 2,
                Name = TestingActivityName,
                LocationId = TestingLocationId,
            });
            await this.Context.SaveChangesAsync();

            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();
            await this.ticketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels);

            var expectedUsersShoppingCartItemsCount = 0;
            var actualUsersShoppingCartItemsCount = this.Context.ShoppingCartActivities.Where(sca => sca.ShoppingCart.UserId == TestUsername)?.Count();

            Assert.Equal(expectedUsersShoppingCartItemsCount, actualUsersShoppingCartItemsCount);
        }

        [Fact]
        public async Task BookAllAsyncThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.AddTestingActivityToDb();
            await this.AddTestingUserToDb();

            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ticketsServiceMock.BookAllAsync(NotExistingUsername, shoppingCartActivityViewModels));

            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, NotExistingUsername), exception.Message);
        }

        [Fact]
        public async Task BookAllAsyncThrowsNullReferenceExceptionIfActivityNotFound()
        {
            await this.AddTestingUserToDb();
            var shoppingCartActivityViewModels = this.GetShoppingCartActivityViewModels();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.ticketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceActivityId, TestingActivityId), exception.Message);
        }

        [Fact]
        public async Task BookAllAsyncThrowsArgumentExceptionIfZeroOrNegativeQuantity()
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

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                this.ticketsServiceMock.BookAllAsync(TestUsername, shoppingCartActivityViewModels));
            Assert.Equal(ServicesDataConstants.ZeroOrNegativeQuantity, exception.Message);
        }

        [Fact]
        public async Task GetDetailsAsyncReturnsCorrectTicketDetails()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            this.Context.Tickets.Add(new Ticket
            {
                Id = TestingTicketId,
                ActivityId = TestingActivityId,
                UserId = testingUserId,
            });
            await this.Context.SaveChangesAsync();

            var expected = this.Context.Tickets.OrderBy(t => t.CreatedOn);
            var actual = await this.ticketsServiceMock.GetDetailsAsync(TestingTicketId);

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

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.ticketsServiceMock.GetDetailsAsync(TestingTicketId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceTicketId, TestingTicketId), exception.Message);
        }

        [Fact]
        public async Task GetAllAsyncReturnsAllTicketsForTheUser()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingActivityToDb();

            this.Context.Tickets.Add(new Ticket
            {
                Id = 1,
                UserId = testingUserId,
                ActivityId = TestingActivityId,
            });
            this.Context.Tickets.Add(new Ticket
            {
                Id = 2,
                UserId = testingUserId,
                ActivityId = TestingActivityId,
            });
            await this.Context.SaveChangesAsync();

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

            var actual = await this.ticketsServiceMock.GetAllAsync(TestUsername);
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

            this.Context.Tickets.Add(new Ticket
            {
                Id = 1,
                UserId = testingUserId,
                ActivityId = TestingActivityId,
            });
            this.Context.Tickets.Add(new Ticket
            {
                Id = 2,
                UserId = Guid.NewGuid().ToString(),
                ActivityId = TestingActivityId,
            });
            this.Context.Tickets.Add(new Ticket
            {
                Id = 3,
                UserId = Guid.NewGuid().ToString(),
                ActivityId = TestingActivityId,
            });
            await this.Context.SaveChangesAsync();

            var expected = new TicketDetailsViewModel[]
            {
                new TicketDetailsViewModel
                {
                    Id = 1,
                    UserId = testingUserId,
                    ActivityId = TestingActivityId,
                },
            };

            var actual = await this.ticketsServiceMock.GetAllAsync(TestUsername);
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
            this.Context.Users.Add(new ApplicationUser
            {
                Id = testingUserId,
                UserName = TestUsername
            });
            await this.Context.SaveChangesAsync();
        }

        private async Task AddTestingActivityToDb()
        {
            await this.AddTestingLocationToDb();
            this.Context.Activities.Add(new Activity
            {
                Id = TestingActivityId,
                Name = TestingActivityName,
                LocationId = TestingLocationId,
            });
            await this.Context.SaveChangesAsync();
        }

        private async Task AddTestingLocationToDb()
        {
            this.Context.Locations.Add(new Location
            {
                Id = TestingLocationId,
                Name = TestingLocationName,
            });
            await this.Context.SaveChangesAsync();
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

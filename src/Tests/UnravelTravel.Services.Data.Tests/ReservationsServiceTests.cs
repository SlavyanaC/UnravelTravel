namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using AutoMapper;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.InputModels.Account;
    using UnravelTravel.Models.InputModels.Reservations;
    using UnravelTravel.Models.ViewModels.Reservations;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;
    using Xunit;

    public class ReservationsServiceTests : BaseServiceTests
    {
        private const string TestUsername = "Pesho";
        private const string TestRestaurantName = "Pri Ivan";
        private const int TestRestaurantId = 1;
        private const string NotExistingUsername = "Stamat";
        private const int TestReservationId = 1;
        private const int TestPeopleCount = 2;

        private readonly string testUserId = Guid.NewGuid().ToString();
        private readonly DateTime testReservationDateTime = DateTime.Now.AddDays(2);

        private readonly IReservationsService reservationsServiceMock;

        public ReservationsServiceTests()
        {
            //// If I move this to Base class AutoMapper throws exception on strange places
            //Mapper.Reset();
            //AutoMapperConfig.RegisterMappings(typeof(LoginInputModel).GetTypeInfo().Assembly);

            this.reservationsServiceMock = this.Provider.GetRequiredService<IReservationsService>();
        }

        [Fact]
        public async Task BookAsyncCreatesReservation()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingRestaurantToDb();
            var reservationCreateInputModel = this.GetTestingReservationCreateInputModel();
            var reservationViewModel = await this.reservationsServiceMock.BookAsync(TestRestaurantId, TestUsername, reservationCreateInputModel);

            var reservationsRepository = this.Context.Reservations.OrderBy(r => r.CreatedOn);

            Assert.Collection(reservationsRepository,
                elem1 =>
                {
                    Assert.Equal(reservationsRepository.Last().Id, reservationViewModel.Id);
                    Assert.Equal(reservationsRepository.Last().UserId, reservationViewModel.UserId);
                    Assert.Equal(reservationsRepository.Last().Date, reservationViewModel.Date);
                    Assert.Equal(reservationsRepository.Last().PeopleCount, reservationViewModel.PeopleCount);
                });
            Assert.Equal(1, reservationsRepository.Count());
        }

        [Fact]
        public async Task BookAsyncThrowsNullReferenceExceptionIfUserNotFound()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingRestaurantToDb();

            var reservationCreateInputModel = this.GetTestingReservationCreateInputModel();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.reservationsServiceMock.BookAsync(TestRestaurantId, NotExistingUsername, reservationCreateInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceUsername, NotExistingUsername), exception.Message);
        }

        [Fact]
        public async Task BookAsyncThrowsNulReferenceExceptionIfRestaurantNotFound()
        {
            await this.AddTestingUserToDb();
            var reservationCreateInputModel = this.GetTestingReservationCreateInputModel();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() => this.reservationsServiceMock.BookAsync(TestRestaurantId, TestUsername, reservationCreateInputModel));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceRestaurantId, TestRestaurantId), exception.Message);
        }

        [Fact]
        public async Task BookAsyncAddsPeopleCountToExistingReservation()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingRestaurantToDb();

            this.Context.Reservations.Add(new Reservation
            {
                Id = TestReservationId,
                Date = testReservationDateTime,
                PeopleCount = TestPeopleCount,
                RestaurantId = TestRestaurantId,
                UserId = testUserId,
            });
            await this.Context.SaveChangesAsync();

            var reservationCreateInputModel = this.GetTestingReservationCreateInputModel();
            var reservationViewModel = await this.reservationsServiceMock.BookAsync(TestRestaurantId, TestUsername, reservationCreateInputModel);

            var reservationsRepository = this.Context.Reservations.OrderBy(r => r.CreatedOn);

            Assert.Collection(reservationsRepository,
                elem1 =>
                {
                    Assert.Equal(reservationsRepository.Last().Id, reservationViewModel.Id);
                    Assert.Equal(reservationsRepository.Last().UserId, reservationViewModel.UserId);
                    Assert.Equal(reservationsRepository.Last().Date, reservationViewModel.Date);
                    Assert.Equal(reservationsRepository.Last().PeopleCount, TestPeopleCount + TestPeopleCount);
                });
            Assert.Equal(1, reservationsRepository.Count());
        }

        [Fact]
        public async Task GetDetailsAsyncReturnsCorrectReservationDetails()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingRestaurantToDb();

            this.Context.Reservations.Add(new Reservation
            {
                Id = TestReservationId,
                Date = testReservationDateTime,
                PeopleCount = TestPeopleCount,
                RestaurantId = TestRestaurantId,
                UserId = testUserId,
            });
            await this.Context.SaveChangesAsync();

            var reservationDetailsViewModel = await this.reservationsServiceMock.GetDetailsAsync(TestReservationId);
            var reservationsRepository = this.Context.Reservations.OrderBy(r => r.CreatedOn);

            Assert.Collection(reservationsRepository,
                elem1 =>
                {
                    Assert.Equal(reservationsRepository.Last().Id, reservationDetailsViewModel.Id);
                    Assert.Equal(reservationsRepository.Last().UserId, reservationDetailsViewModel.UserId);
                    Assert.Equal(reservationsRepository.Last().Date, reservationDetailsViewModel.Date);
                    Assert.Equal(reservationsRepository.Last().PeopleCount, reservationDetailsViewModel.PeopleCount);
                });
            Assert.Equal(1, reservationsRepository.Count());
        }

        [Fact]
        public async Task GetDetailsAsyncThrowsNullReferenceExceptionIfReservationNotFound()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingRestaurantToDb();

            var exception = await Assert.ThrowsAsync<NullReferenceException>(() =>
                this.reservationsServiceMock.GetDetailsAsync(TestReservationId));
            Assert.Equal(string.Format(ServicesDataConstants.NullReferenceReservationId, TestReservationId), exception.Message);
        }

        [Fact]
        public async Task GetAllAsyncReturnsAllReservationForTheUser()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingRestaurantToDb();

            this.Context.Reservations.Add(new Reservation
            {
                Id = 1,
                UserId = testUserId,
                RestaurantId = TestRestaurantId,
                Date = testReservationDateTime,
                PeopleCount = TestPeopleCount,
            });
            this.Context.Reservations.Add(new Reservation
            {
                Id = 2,
                UserId = testUserId,
                RestaurantId = TestRestaurantId,
                Date = testReservationDateTime.AddDays(1),
                PeopleCount = TestPeopleCount,
            });
            await this.Context.SaveChangesAsync();

            var expected = new ReservationDetailsViewModel[]
            {
                new ReservationDetailsViewModel
                {
                    Id = 1,
                    UserId = testUserId,
                    RestaurantId = TestRestaurantId,
                    Date = testReservationDateTime,
                    PeopleCount = TestPeopleCount,
                },
                new ReservationDetailsViewModel()
                {
                    Id = 2,
                    UserId = testUserId,
                    RestaurantId = TestRestaurantId,
                    Date = testReservationDateTime.AddDays(1),
                    PeopleCount = TestPeopleCount,
                }
            };

            var actual = await this.reservationsServiceMock.GetAllAsync(TestUsername);
            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].UserId, elem1.UserId);
                    Assert.Equal(expected[0].RestaurantId, elem1.RestaurantId);
                    Assert.Equal(expected[0].Date, elem1.Date);
                    Assert.Equal(expected[0].PeopleCount, elem1.PeopleCount);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].UserId, elem2.UserId);
                    Assert.Equal(expected[1].RestaurantId, elem2.RestaurantId);
                    Assert.Equal(expected[1].Date, elem2.Date);
                    Assert.Equal(expected[1].PeopleCount, elem2.PeopleCount);
                });
            Assert.Equal(expected.Length, actual.Length);
        }

        [Fact]
        public async Task GetAllAsyncDoesNotReturnAnotherUsersReservations()
        {
            await this.AddTestingUserToDb();
            await this.AddTestingRestaurantToDb();

            this.Context.Reservations.Add(new Reservation
            {
                Id = 1,
                UserId = testUserId,
                RestaurantId = TestRestaurantId,
                Date = testReservationDateTime,
                PeopleCount = TestPeopleCount,
            });
            this.Context.Reservations.Add(new Reservation
            {
                Id = 2,
                UserId = Guid.NewGuid().ToString(),
                RestaurantId = TestRestaurantId,
                Date = testReservationDateTime.AddDays(1),
                PeopleCount = TestPeopleCount,
            });
            this.Context.Reservations.Add(new Reservation
            {
                Id = 3,
                UserId = Guid.NewGuid().ToString(),
                RestaurantId = TestRestaurantId,
                Date = testReservationDateTime.AddDays(1),
                PeopleCount = TestPeopleCount,
            });
            await this.Context.SaveChangesAsync();

            var expected = new ReservationDetailsViewModel[]
            {
                new ReservationDetailsViewModel
                {
                    Id = 1,
                    UserId = testUserId,
                    RestaurantId = TestRestaurantId,
                    Date = testReservationDateTime,
                    PeopleCount = TestPeopleCount,
                },
            };

            var actual = await this.reservationsServiceMock.GetAllAsync(TestUsername);
            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].UserId, elem1.UserId);
                    Assert.Equal(expected[0].RestaurantId, elem1.RestaurantId);
                    Assert.Equal(expected[0].Date, elem1.Date);
                    Assert.Equal(expected[0].PeopleCount, elem1.PeopleCount);
                });
            Assert.Equal(expected.Length, actual.Length);
        }

        private ReservationCreateInputModel GetTestingReservationCreateInputModel()
        {
            return new ReservationCreateInputModel
            {
                Date = this.testReservationDateTime,
                PeopleCount = TestPeopleCount,
            };
        }

        private async Task AddTestingRestaurantToDb()
        {
            this.Context.Restaurants.Add(new Restaurant
            {
                Id = TestRestaurantId,
                Name = TestRestaurantName,
            });
            await this.Context.SaveChangesAsync();
        }

        private async Task AddTestingUserToDb()
        {
            this.Context.Users.Add(new ApplicationUser
            {
                Id = testUserId,
                UserName = TestUsername
            });
            await this.Context.SaveChangesAsync();
        }
    }
}

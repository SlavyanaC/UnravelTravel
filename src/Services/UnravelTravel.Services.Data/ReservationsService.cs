namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.InputModels.Reservations;
    using UnravelTravel.Models.ViewModels.Reservations;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    using AutoMap = AutoMapper;

    public class ReservationsService : IReservationsService
    {
        private readonly IRepository<Reservation> reservationsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Restaurant> restaurantsRepository;

        public ReservationsService(IRepository<Reservation> reservationsRepository, IRepository<ApplicationUser> usersRepository, IRepository<Restaurant> restaurantsRepository)
        {
            this.usersRepository = usersRepository;
            this.restaurantsRepository = restaurantsRepository;
            this.reservationsRepository = reservationsRepository;
        }

        public async Task<ReservationDetailsViewModel> BookAsync(int restaurantId, string username, ReservationCreateInputModel reservationCreateInputModel)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var restaurant = await this.restaurantsRepository.All().FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceRestaurantId, restaurantId));
            }

            var reservation = await this.reservationsRepository.All().FirstOrDefaultAsync(r =>
                r.User == user && r.Restaurant == restaurant && r.Date == reservationCreateInputModel.Date);
            if (reservation != null)
            {
                reservation.PeopleCount += reservationCreateInputModel.PeopleCount;
                this.reservationsRepository.Update(reservation);
            }
            else
            {
                reservation = new Reservation
                {
                    User = user,
                    Restaurant = restaurant,
                    Date = reservationCreateInputModel.Date,
                    PeopleCount = reservationCreateInputModel.PeopleCount,
                };

                this.reservationsRepository.Add(reservation);
            }

            await this.reservationsRepository.SaveChangesAsync();

            var reservationDetailsViewModel = AutoMap.Mapper.Map<ReservationDetailsViewModel>(reservation);
            return reservationDetailsViewModel;
        }

        public async Task<ReservationDetailsViewModel> GetDetailsAsync(int id)
        {
            var reservationDetailsViewModel = await this.reservationsRepository
                .All()
                .To<ReservationDetailsViewModel>()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservationDetailsViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceReservationId, id));
            }

            return reservationDetailsViewModel;
        }

        public async Task<ReservationDetailsViewModel[]> GetAllAsync(string username)
        {
            var allReservations = await this.reservationsRepository
                .All()
                .Where(r => r.User.UserName == username)
                .To<ReservationDetailsViewModel>()
                .ToArrayAsync();

            return allReservations;
        }
    }
}

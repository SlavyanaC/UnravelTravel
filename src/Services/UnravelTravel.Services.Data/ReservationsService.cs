namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Reservations;
    using UnravelTravel.Services.Mapping;

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

        public async Task<int> BookAsync(int restaurantId, string username, DateTime reservationDate, int peopleCount)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            var restaurant = await this.restaurantsRepository.All().FirstOrDefaultAsync(r => r.Id == restaurantId);

            var reservation = new Reservation
            {
                User = user,
                Restaurant = restaurant,
                Date = reservationDate,
                PeopleCount = peopleCount,
            };

            this.reservationsRepository.Add(reservation);
            await this.reservationsRepository.SaveChangesAsync();

            return reservation.Id;
        }

        public async Task<ReservationDetailsViewModel> GetDetailsAsync(int id)
        {
            var reservationDetailsViewModel = await this.reservationsRepository
                .All()
                .To<ReservationDetailsViewModel>()
                .FirstOrDefaultAsync(r => r.Id == id);

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

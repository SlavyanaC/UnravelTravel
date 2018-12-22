namespace UnravelTravel.Services.Data.Contracts
{
    using System;
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.Reservations;

    public interface IReservationsService
    {
        Task<int> BookAsync(int restaurantId, string username, DateTime reservationDate, int peopleCount);

        Task<ReservationDetailsViewModel> GetDetailsAsync(int id);

        Task<ReservationDetailsViewModel[]> GetAllAsync(string username);
    }
}

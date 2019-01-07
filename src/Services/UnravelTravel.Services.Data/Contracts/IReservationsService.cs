namespace UnravelTravel.Services.Data.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.Reservations;
    using UnravelTravel.Models.ViewModels.Reservations;

    public interface IReservationsService
    {
        Task<ReservationDetailsViewModel> BookAsync(int restaurantId, string username, ReservationCreateInputModel reservationCreateInputModel);

        Task<ReservationDetailsViewModel> GetDetailsAsync(int id);

        Task<IEnumerable<ReservationDetailsViewModel>> GetAllAsync(string username);
    }
}

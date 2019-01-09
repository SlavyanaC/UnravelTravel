namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.Reservations;
    using UnravelTravel.Models.ViewModels.Reservations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Filters;

    public class ReservationsController : BaseController
    {
        private readonly IReservationsService reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            this.reservationsService = reservationsService;
        }

        public IActionResult Book(int id) => this.View();

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Book(int id, ReservationCreateInputModel reservationCreateInputModel)
        {
            var userIdentifier = reservationCreateInputModel.GuestUserEmail ?? this.User.Identity.Name;
            var reservation = await this.reservationsService.BookAsync(id, userIdentifier, reservationCreateInputModel);
            return this.View("_BookingConfirmation");
            //return this.RedirectToAction(nameof(this.Index));
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            ReservationDetailsViewModel reservationDetailsViewModel;
            try
            {
                reservationDetailsViewModel = await this.reservationsService.GetDetailsAsync(id);

                // TODO: It would be better if reservation id as GUID -> parameter tampering would be impossible
                if (reservationDetailsViewModel.User == null || reservationDetailsViewModel.User.UserName != this.User.Identity.Name)
                {
                    return this.View("_AccessDenied");
                }
            }
            catch (NullReferenceException)
            {
                return this.View("_AccessDenied");
            }

            return this.View(reservationDetailsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var username = this.User.Identity.Name;
            var userReservationsViewModel = await this.reservationsService.GetAllAsync(username);
            return this.View(userReservationsViewModel);
        }
    }
}

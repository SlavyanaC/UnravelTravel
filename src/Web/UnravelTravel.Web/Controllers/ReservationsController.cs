namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.Reservations;
    using UnravelTravel.Models.ViewModels.Reservations;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Filters;

    public class ReservationsController : BaseController
    {
        private readonly IReservationsService reservationsService;
        private readonly IRestaurantsService restaurantsService;

        public ReservationsController(IReservationsService reservationsService, IRestaurantsService restaurantsService)
        {
            this.reservationsService = reservationsService;
            this.restaurantsService = restaurantsService;
        }

        public async Task<IActionResult> BookPartial(int id)
        {
            var restaurant = await this.restaurantsService.GetViewModelByIdAsync<RestaurantDetailsViewModel>(id);
            return this.PartialView("_BookPartial", restaurant);
        }

        //[HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<JsonResult> Book(int restaurantId, ReservationCreateInputModel reservationCreateInputModel)
        {
            var userIdentifier = reservationCreateInputModel.GuestUserEmail ?? this.User.Identity.Name;
            var reservation = await this.reservationsService.BookAsync(restaurantId, userIdentifier, reservationCreateInputModel);
            var result = new JsonResult(new { success = 1 });
            return result;
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

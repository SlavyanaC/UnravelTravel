namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.InputModels.Reservations;

    [Authorize(Roles = "User")]
    public class ReservationsController : BaseController
    {
        private readonly IReservationsService reservationsService;

        public ReservationsController(IReservationsService reservationsService)
        {
            this.reservationsService = reservationsService;
        }

        public IActionResult Book(int id)
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Book(int id, ReservationCreateInputModel reservationCreateInputModel)
        {
            var username = this.User.Identity.Name;
            var reservationId = this.reservationsService.BookAsync(
                     id,
                     username,
                     reservationCreateInputModel.Date,
                     reservationCreateInputModel.PeopleCount)
                 .GetAwaiter()
                 .GetResult();
            return this.RedirectToAction("Details", new { id });
        }

        public IActionResult Details(int id)
        {
            var detailViewModel = this.reservationsService.GetDetailsAsync(id)
                .GetAwaiter()
                .GetResult();

            return this.View(detailViewModel);
        }

        public IActionResult All()
        {
            var username = this.User.Identity.Name;
            var userReservationsViewModel = this.reservationsService.GetAllAsync(username)
                .GetAwaiter()
                .GetResult();
            return this.View(userReservationsViewModel);
        }
    }
}

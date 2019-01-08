namespace UnravelTravel.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.Reservations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Filters;

    [Authorize]
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
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Book(int id, ReservationCreateInputModel reservationCreateInputModel)
        {
            var username = this.User.Identity.Name;
            var reservation = await this.reservationsService.BookAsync(id, username, reservationCreateInputModel);
            return this.RedirectToAction(nameof(this.All));
        }

        public async Task<IActionResult> Details(int id)
        {
            var detailViewModel = await this.reservationsService.GetDetailsAsync(id);
            return this.View(detailViewModel);
        }

        public async Task<IActionResult> All()
        {
            var username = this.User.Identity.Name;
            var userReservationsViewModel = await this.reservationsService.GetAllAsync(username);
            return this.View(userReservationsViewModel);
        }
    }
}

namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;

    [Authorize(Roles = "User")]
    public class TicketsController : BaseController
    {
        private readonly ITicketsService ticketsService;
        private readonly IShoppingCartsService shoppingCartsService;

        public TicketsController(ITicketsService ticketsService, IShoppingCartsService shoppingCartsService)
        {
            this.ticketsService = ticketsService;
            this.shoppingCartsService = shoppingCartsService;
        }

        [HttpPost]
        public IActionResult Book()
        {
            var username = this.User.Identity.Name;
            var shoppingCartActivities = this.shoppingCartsService.GetAllTickets(username).GetAwaiter().GetResult();
            this.ticketsService.BookAllAsync(username, shoppingCartActivities).GetAwaiter().GetResult();
            return this.RedirectToAction("All");
        }

        public IActionResult Details(int id)
        {
            var ticketDetailsViewModel = this.ticketsService.GetDetailsAsync(id)
                .GetAwaiter()
                .GetResult();

            return this.View(ticketDetailsViewModel);
        }

        public IActionResult All()
        {
            var username = this.User.Identity.Name;
            var tickets = this.ticketsService.GetAllAsync(username)
                .GetAwaiter()
                .GetResult();
            return this.View(tickets);
        }
    }
}

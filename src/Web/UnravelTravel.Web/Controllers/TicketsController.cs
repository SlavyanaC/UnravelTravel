namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;

    [Authorize(Roles = "User")]
    public class TicketsController : BaseController
    {
        private readonly ITicketsService ticketsService;

        public TicketsController(ITicketsService ticketsService)
        {
            this.ticketsService = ticketsService;
        }

        [HttpPost]
        public IActionResult Book(int id)
        {
            var username = this.User.Identity.Name;
            var ticketId = this.ticketsService.BookAsync(username, id)
                 .GetAwaiter()
                 .GetResult();
            return this.RedirectToAction("Details", new { id = ticketId });
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

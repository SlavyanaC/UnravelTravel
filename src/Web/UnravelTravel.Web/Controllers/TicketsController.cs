namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Common;
    using UnravelTravel.Models.ViewModels.Tickets;
    using UnravelTravel.Services.Data.Contracts;

    [Authorize]
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
        public async Task<IActionResult> Book()
        {
            var userName = this.User.Identity.Name;
            var shoppingCartActivities = await this.shoppingCartsService.GetAllShoppingCartActivitiesAsync(userName);
            await this.ticketsService.BookAllAsync(userName, shoppingCartActivities.ToArray(), GlobalConstants.CashPaymentMethod);
            return this.RedirectToAction("All");
        }

        public async Task<IActionResult> Details(int id)
        {
            TicketDetailsViewModel ticketDetailsViewModel;
            try
            {
                ticketDetailsViewModel = await this.ticketsService.GetDetailsAsync(id);

                // TODO: It would be better if ticket id as GUID -> parameter tampering would be impossible
                if (ticketDetailsViewModel.User == null || ticketDetailsViewModel.User.UserName != this.User.Identity.Name)
                {
                    return this.View("_AccessDenied");
                }
            }
            catch (NullReferenceException)
            {
                return this.View("_AccessDenied");
            }

            return this.View(ticketDetailsViewModel);
        }

        public async Task<IActionResult> All()
        {
            var username = this.User.Identity.Name;
            var tickets = await this.ticketsService.GetAllAsync(username);
            return this.View(tickets);
        }
    }
}

namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Stripe;
    using UnravelTravel.Common;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;
    using UnravelTravel.Web.Helpers;

    public class ShoppingCartController : BaseController
    {
        private readonly IShoppingCartsService shoppingCartService;
        private readonly ITicketsService ticketsService;

        public ShoppingCartController(IShoppingCartsService shoppingCartService, ITicketsService ticketsService)
        {
            this.shoppingCartService = shoppingCartService;
            this.ticketsService = ticketsService;
        }

        public async Task<IActionResult> Index()
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                var shoppingCart = await this.shoppingCartService.GetAllShoppingCartActivitiesAsync(username);
                return this.View(shoppingCart);
            }

            var cart = this.GetShoppingCartFromSession();
            return this.View(cart);
        }

        public async Task<IActionResult> Add(int id, int quantity)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                await this.shoppingCartService.AddActivityToShoppingCartAsync(id, username, quantity);
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                var shoppingCartActivity = await this.shoppingCartService.GetGuestShoppingCartActivityToAdd(id, quantity);

                var cartAsList = cart.ToList();
                cartAsList.Add(shoppingCartActivity);
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cartAsList);
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id, int newQuantity)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                await this.shoppingCartService.EditShoppingCartActivityAsync(id, username, newQuantity);
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                cart = this.shoppingCartService.EditGuestShoppingCartActivity(id, cart.ToArray(), newQuantity).ToArray();
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cart);
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                await this.shoppingCartService.DeleteActivityFromShoppingCart(id, username);
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                cart = this.shoppingCartService.DeleteActivityFromGuestShoppingCart(id, cart.ToArray()).ToArray();
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cart);
            }

            return this.RedirectToAction("Index");
        }

        // Stripe
        public async Task<IActionResult> Charge(string stripeEmail, string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken,
            });

            IEnumerable<ShoppingCartActivityViewModel> userTickets;
            var userIdentifier = this.User.Identity.Name ?? stripeEmail;
            if (this.User.Identity.Name != null)
            {
                userTickets = await this.shoppingCartService.GetAllShoppingCartActivitiesAsync(userIdentifier);
            }
            else
            {
                userTickets = this.GetShoppingCartFromSession();
            }

            var totalSum = userTickets.Sum(ut => ut.ShoppingCartActivityTotalPrice);
            var totalSumInCents = totalSum * 100;

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = (long)totalSumInCents,
                Description = $"{userIdentifier} bought {userTickets.Count()} ticket on {DateTime.UtcNow}",
                Currency = "usd",
                CustomerId = customer.Id,
                ReceiptEmail = stripeEmail,
            });

            await this.ticketsService.BookAllAsync(userIdentifier, userTickets.ToArray(), GlobalConstants.OnlinePaymentMethod);
            this.HttpContext.Session.Clear(); // TODO: Make sure this is ok

            return this.User.Identity.Name != null ?
                this.RedirectToAction(actionName: "All", controllerName: "Tickets") :
                this.RedirectToAction(nameof(this.Index));
        }

        private IEnumerable<ShoppingCartActivityViewModel> GetShoppingCartFromSession()
        {
            return this.HttpContext.Session
                   .GetObjectFromJson<ShoppingCartActivityViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                   new List<ShoppingCartActivityViewModel>().ToArray();
        }
    }
}

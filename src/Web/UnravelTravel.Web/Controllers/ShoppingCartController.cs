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

        public ShoppingCartController(IShoppingCartsService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
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

                // TODO: Work with List<ShoppingCartActivityViewModel> instead of array
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
                cart = this.shoppingCartService.EditGuestShoppingCartActivity(id, cart, newQuantity).ToArray();
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
                cart = this.shoppingCartService.DeleteActivityFromGuestShoppingCart(id, cart).ToArray();
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

            var username = this.User.Identity.Name;
            var userTickets = await this.shoppingCartService.GetAllShoppingCartActivitiesAsync(username);
            var totalSum = userTickets.Sum(ut => ut.ShoppingCartActivityTotalPrice);
            var totalSumInCents = totalSum * 100;

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = (long)totalSumInCents,
                Description = $"{username} bought {userTickets.Count()} ticket on {DateTime.UtcNow}",
                Currency = "usd",
                CustomerId = customer.Id,
                ReceiptEmail = stripeEmail,
            });

            return this.RedirectToAction("BookGet", "Tickets");
        }

        private ShoppingCartActivityViewModel[] GetShoppingCartFromSession()
        {
            return this.HttpContext.Session
                   .GetObjectFromJson<ShoppingCartActivityViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                   new List<ShoppingCartActivityViewModel>().ToArray();
        }
    }
}

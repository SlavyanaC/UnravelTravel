namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        public IActionResult Index()
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                var shoppingCart = this.shoppingCartService.GetAllTickets(username)
                    .GetAwaiter().GetResult();

                return this.View(shoppingCart);
            }

            var cart = this.GetShoppingCartFromSession();
            return this.View(cart);
        }

        public IActionResult Add(int id, int quantity)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                this.shoppingCartService.AddActivityToShoppingCart(id, username, quantity).GetAwaiter().GetResult();
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                var shoppingCartActivity = this.shoppingCartService.GetGuestShoppingCartActivityToAdd(id, quantity)
                     .GetAwaiter()
                     .GetResult();

                // TODO: Work with List<ShoppingCartActivityViewModel> instead of array
                var cartAsList = cart.ToList();
                cartAsList.Add(shoppingCartActivity);
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cartAsList);
            }

            return this.RedirectToAction("Index");
        }

        public IActionResult Edit(int id, int newQuantity)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                this.shoppingCartService.EditShoppingCartActivity(id, username, newQuantity).GetAwaiter().GetResult();
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                cart = this.shoppingCartService.EditGuestShoppingCartActivity(id, cart, newQuantity);

                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cart);
            }

            return this.RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                this.shoppingCartService.DeleteActivityFromShoppingCart(id, username).GetAwaiter().GetResult();
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                cart = this.shoppingCartService.DeleteActivityFromGuestShoppingCart(id, cart);
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cart);
            }

            return this.RedirectToAction("Index");
        }

        // Stripe
        public IActionResult Charge(string stripeEmail, string stripeToken)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                SourceToken = stripeToken,
            });

            var username = this.User.Identity.Name;
            var userTickets = this.shoppingCartService.GetAllTickets(username).GetAwaiter().GetResult();
            var totalSum = userTickets.Sum(ut => ut.ShoppingCartActivityTotalPrice);
            var totalSumInCents = totalSum * 100;

            var charge = charges.Create(new ChargeCreateOptions
            {
                Amount = (long)totalSumInCents,
                Description = $"{username} bought {userTickets.Count()} ticket on {DateTime.UtcNow}",
                Currency = "usd",
                CustomerId = customer.Id,
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

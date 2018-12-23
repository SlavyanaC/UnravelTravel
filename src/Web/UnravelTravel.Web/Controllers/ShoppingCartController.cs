namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Stripe;
    using UnravelTravel.Services.Data.Contracts;

    [Authorize]
    public class ShoppingCartController : BaseController
    {
        private readonly IShoppingCartsService shoppingCartService;

        public ShoppingCartController(IShoppingCartsService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        public IActionResult Index()
        {
            var username = this.User.Identity.Name;
            var shoppingCartTickets = this.shoppingCartService.GetAllTickets(username)
                .GetAwaiter().GetResult();

            return this.View(shoppingCartTickets);
        }

        public IActionResult Add(int id, int quantity)
        {
            var username = this.User.Identity.Name;
            this.shoppingCartService.AddActivityToShoppingCart(id, username, quantity).GetAwaiter().GetResult();
            return this.RedirectToAction("Index");
        }

        public IActionResult Edit(int id, int newQuantity)
        {
            var username = this.User.Identity.Name;
            this.shoppingCartService.EditShoppingCartActivity(id, username, newQuantity).GetAwaiter().GetResult();
            return this.RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var username = this.User.Identity.Name;
            this.shoppingCartService.DeleteActivityFromShoppingCart(id, username).GetAwaiter().GetResult();
            return this.RedirectToAction("Index");
        }

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
    }
}

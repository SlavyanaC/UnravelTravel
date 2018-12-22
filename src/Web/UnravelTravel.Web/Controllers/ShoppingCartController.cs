namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
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
    }
}

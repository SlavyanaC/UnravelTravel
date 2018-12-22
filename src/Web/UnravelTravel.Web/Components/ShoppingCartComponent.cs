namespace UnravelTravel.Web.Components
{
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;

    public class ShoppingCartComponent : ViewComponent
    {
        private readonly IShoppingCartsService shoppingCartsService;

        public ShoppingCartComponent(IShoppingCartsService shoppingCartsService)
        {
            this.shoppingCartsService = shoppingCartsService;
        }

        public IViewComponentResult Invoke()
        {
            var username = this.User.Identity.Name;
            var shoppingCartTickets = this.shoppingCartsService.GetAllTickets(username)
                .GetAwaiter()
                .GetResult();

            return this.View(shoppingCartTickets);
        }
    }
}

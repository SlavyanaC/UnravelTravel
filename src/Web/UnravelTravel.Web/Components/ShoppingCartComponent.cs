namespace UnravelTravel.Web.Components
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Common;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;
    using UnravelTravel.Web.Helpers;

    public class ShoppingCartComponent : ViewComponent
    {
        private readonly IShoppingCartsService shoppingCartsService;

        public ShoppingCartComponent(IShoppingCartsService shoppingCartsService)
        {
            this.shoppingCartsService = shoppingCartsService;
        }

        public IViewComponentResult Invoke()
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                var shoppingCartTickets = this.shoppingCartsService.GetAllShoppingCartActivitiesAsync(username)
                    .GetAwaiter()
                    .GetResult();

                return this.View(shoppingCartTickets);
            }

            var cart = this.HttpContext.Session
                       .GetObjectFromJson<ShoppingCartActivityViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                       new List<ShoppingCartActivityViewModel>().ToArray();

            return this.View(cart);
        }
    }
}

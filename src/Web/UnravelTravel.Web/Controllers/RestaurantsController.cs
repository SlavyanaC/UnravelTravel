namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;

    public class RestaurantsController : BaseController
    {
        private readonly IRestaurantsService restaurantsService;

        public RestaurantsController(IRestaurantsService restaurantsService)
        {
            this.restaurantsService = restaurantsService;
        }

        public IActionResult All()
        {
            var destinations = this.restaurantsService.GetAllRestaurantsAsync()
                .GetAwaiter()
                .GetResult();
            if (destinations == null)
            {
                return this.Redirect("/");
            }

            return this.View(destinations);
        }
    }
}
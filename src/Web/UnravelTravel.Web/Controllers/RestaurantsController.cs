namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.Restaurants;
    using UnravelTravel.Models.ViewModels.Restaurants;
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
            var destinations = this.restaurantsService.GetAllAsync().GetAwaiter().GetResult();
            if (destinations == null)
            {
                return this.Redirect("/");
            }

            return this.View(destinations);
        }

        public IActionResult Details(int id)
        {
            var restaurantViewModel = this.restaurantsService.GetViewModelByIdAsync<RestaurantDetailsViewModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(restaurantViewModel);
        }

        [Authorize]
        public IActionResult Review(int id)
        {
            var reviewInputModel = this.restaurantsService.GetViewModelByIdAsync<RestaurantReviewInputModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(reviewInputModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Review(int id, RestaurantReviewInputModel restaurantReviewInputModel)
        {
            var username = this.User.Identity.Name;
            this.restaurantsService.Review(id, username, restaurantReviewInputModel).GetAwaiter().GetResult();
            return this.RedirectToAction("Details", new { id });
        }
    }
}

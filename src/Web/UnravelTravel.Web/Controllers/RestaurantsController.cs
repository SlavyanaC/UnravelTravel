namespace UnravelTravel.Web.Controllers
{
    using System;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Restaurants;
    using UnravelTravel.Web.InputModels.Restaurants;

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

        public IActionResult Details(int id)
        {
            var restaurantViewModel = this.restaurantsService.GetViewModelAsync<RestaurantDetailsViewModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(restaurantViewModel);
        }

        [Authorize]
        public IActionResult Review(int id)
        {
            var reviewInputModel = this.restaurantsService.GetViewModelAsync<RestaurantReviewInputModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(reviewInputModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Review(int id, RestaurantReviewInputModel restaurantReviewInputModel)
        {
            var username = this.User.Identity.Name;
            this.restaurantsService.Review(
                id,
                username,
                restaurantReviewInputModel.Rating,
                restaurantReviewInputModel.Content)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("Details", new { id });
        }
    }
}

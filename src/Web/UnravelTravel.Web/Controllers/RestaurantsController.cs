using System;
using Microsoft.Extensions.Caching.Memory;

namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Contracts;

    public class RestaurantsController : BaseController
    {
        private readonly IRestaurantsService restaurantsService;
        private readonly IMemoryCache memoryCache;

        public RestaurantsController(IRestaurantsService restaurantsService, IMemoryCache memoryCache)
        {
            this.restaurantsService = restaurantsService;
            this.memoryCache = memoryCache;
        }

        public IActionResult All()
        {
            if (!this.memoryCache.TryGetValue("AllRestaurantsCache", out RestaurantViewModel[] cacheEntry))
            {
                cacheEntry = this.restaurantsService.GetAllAsync().GetAwaiter().GetResult();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
                this.memoryCache.Set("AllRestaurantsCache", cacheEntry, cacheEntryOptions);
            }

            return this.View(cacheEntry);
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

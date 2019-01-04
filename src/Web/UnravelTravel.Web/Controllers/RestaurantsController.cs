namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;

    public class RestaurantsController : BaseController
    {
        private readonly IRestaurantsService restaurantsService;
        private readonly IMemoryCache memoryCache;

        public RestaurantsController(IRestaurantsService restaurantsService, IMemoryCache memoryCache)
        {
            this.restaurantsService = restaurantsService;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> All()
        {
            //if (!this.memoryCache.TryGetValue(WebConstants.AllRestaurantsCacheKey, out RestaurantViewModel[] cacheEntry))
            //{
            //    cacheEntry = await this.restaurantsService.GetAllAsync();
            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromMinutes(WebConstants.AllViewMinutesExpiration));
            //    this.memoryCache.Set(WebConstants.AllRestaurantsCacheKey, cacheEntry, cacheEntryOptions);
            //}
            //return this.View(cacheEntry);

            var restaurantsViewModel = await this.restaurantsService.GetAllAsync();
            return this.View(restaurantsViewModel);
        }

        public async Task<IActionResult> AllInDestination(int destinationId)
        {
            //if (!this.memoryCache.TryGetValue(WebConstants.AllRestaurantsCacheKey, out RestaurantViewModel[] cacheEntry))
            //{
            //    cacheEntry = await this.restaurantsService.GetAllInDestinationAsync(destinationId);
            //    var cacheEntryOptions = new MemoryCacheEntryOptions()
            //        .SetAbsoluteExpiration(TimeSpan.FromMinutes(WebConstants.AllViewMinutesExpiration));
            //    this.memoryCache.Set(WebConstants.AllRestaurantsCacheKey, cacheEntry, cacheEntryOptions);
            //}
            //return this.View(nameof(this.All), cacheEntry);

            var restaurantsViewModel = await this.restaurantsService.GetAllInDestinationAsync(destinationId);
            return this.View(nameof(this.All), restaurantsViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var restaurantViewModel = await this.restaurantsService.GetViewModelByIdAsync<RestaurantDetailsViewModel>(id);
            return this.View(restaurantViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Review(int id)
        {
            var reviewInputModel = await this.restaurantsService.GetViewModelByIdAsync<RestaurantReviewInputModel>(id);
            return this.View(reviewInputModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Review(int id, RestaurantReviewInputModel restaurantReviewInputModel)
        {
            var username = this.User.Identity.Name;
            await this.restaurantsService.Review(id, username, restaurantReviewInputModel);
            return this.RedirectToAction("Details", new { id });
        }
    }
}

namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;
    using X.PagedList;

    public class RestaurantsController : BaseController
    {
        private readonly IRestaurantsService restaurantsService;
        private readonly IMemoryCache memoryCache;

        public RestaurantsController(IRestaurantsService restaurantsService, IMemoryCache memoryCache)
        {
            this.restaurantsService = restaurantsService;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(RestaurantIndexViewModel restaurantIndexViewModel)
        {
            var restaurants = await this.restaurantsService.GetAllAsync();
            if (restaurantIndexViewModel.SearchString != null)
            {
                restaurants = this.restaurantsService.GetRestaurantsFromSearch(restaurantIndexViewModel.SearchString).ToArray();
            }

            restaurants = this.restaurantsService.SortBy(restaurants, restaurantIndexViewModel.Sorter).ToArray();

            var pageNumber = restaurantIndexViewModel.PageNumber ?? ModelConstants.DefaultPageNumber;
            var pageSize = restaurantIndexViewModel.PageSize ?? ModelConstants.DefaultPageSize;
            var pageDestinationsViewModel = restaurants.ToPagedList(pageNumber, pageSize);

            restaurantIndexViewModel.RestaurantViewModels = pageDestinationsViewModel;

            return this.View(restaurantIndexViewModel);
        }

        public async Task<IActionResult> AllInDestination(RestaurantIndexViewModel restaurantIndexViewModel, int destinationId)
        {
            var restaurants = await this.restaurantsService.GetAllInDestinationAsync(destinationId);
            if (restaurantIndexViewModel.SearchString != null)
            {
                restaurants = this.restaurantsService.GetRestaurantsFromSearch(restaurantIndexViewModel.SearchString).ToArray();
            }

            restaurants = this.restaurantsService.SortBy(restaurants, restaurantIndexViewModel.Sorter).ToArray();

            var pageNumber = restaurantIndexViewModel.PageNumber ?? ModelConstants.DefaultPageNumber;
            var pageSize = restaurantIndexViewModel.PageSize ?? ModelConstants.DefaultPageSize;
            var pageDestinationsViewModel = restaurants.ToPagedList(pageNumber, pageSize);

            restaurantIndexViewModel.RestaurantViewModels = pageDestinationsViewModel;

            return this.View(nameof(this.Index), restaurantIndexViewModel);
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

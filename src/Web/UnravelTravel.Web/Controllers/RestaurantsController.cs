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
    using UnravelTravel.Web.Filters;
    using X.PagedList;

    public class RestaurantsController : BaseController
    {
        private readonly IRestaurantsService restaurantsService;
        private readonly IDestinationsService destinationsService;

        // private readonly IMemoryCache memoryCache;
        public RestaurantsController(IRestaurantsService restaurantsService, IDestinationsService destinationsService/*, IMemoryCache memoryCache*/)
        {
            this.restaurantsService = restaurantsService;
            this.destinationsService = destinationsService;

            // this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(RestaurantIndexViewModel restaurantIndexViewModel)
        {
            var restaurants = await this.restaurantsService.GetAllAsync();
            if (restaurantIndexViewModel.SearchString != null)
            {
                restaurants = this.restaurantsService.GetRestaurantsFromSearch(restaurantIndexViewModel.SearchString, null).ToArray();
            }

            restaurants = this.restaurantsService.SortBy(restaurants.ToArray(), restaurantIndexViewModel.Sorter).ToArray();

            var pageNumber = restaurantIndexViewModel.PageNumber ?? ModelConstants.DefaultPageNumber;
            var pageSize = restaurantIndexViewModel.PageSize ?? ModelConstants.DefaultPageSize;
            var pageDestinationsViewModel = restaurants.ToPagedList(pageNumber, pageSize);

            restaurantIndexViewModel.RestaurantViewModels = pageDestinationsViewModel;

            return this.View(restaurantIndexViewModel);
        }

        public async Task<IActionResult> IndexInDestination(RestaurantIndexViewModel restaurantIndexViewModel, int destinationId)
        {
            var restaurants = await this.restaurantsService.GetAllInDestinationAsync(destinationId);
            if (restaurantIndexViewModel.SearchString != null)
            {
                restaurants = this.restaurantsService.GetRestaurantsFromSearch(restaurantIndexViewModel.SearchString, restaurantIndexViewModel.DestinationId).ToArray();
            }

            restaurants = this.restaurantsService.SortBy(restaurants.ToArray(), restaurantIndexViewModel.Sorter).ToArray();

            var pageNumber = restaurantIndexViewModel.PageNumber ?? ModelConstants.DefaultPageNumber;
            var pageSize = restaurantIndexViewModel.PageSize ?? ModelConstants.DefaultPageSize;
            var pageDestinationsViewModel = restaurants.ToPagedList(pageNumber, pageSize);

            restaurantIndexViewModel.DestinationName = await this.destinationsService.GetDestinationName(restaurantIndexViewModel.DestinationId.Value);
            restaurantIndexViewModel.RestaurantViewModels = pageDestinationsViewModel;

            return this.View(restaurantIndexViewModel);
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
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Review(int id, RestaurantReviewInputModel restaurantReviewInputModel)
        {
            var username = this.User.Identity.Name;
            await this.restaurantsService.Review(id, username, restaurantReviewInputModel);
            return this.RedirectToAction("Details", new { id });
        }
    }
}

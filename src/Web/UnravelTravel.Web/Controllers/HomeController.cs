using UnravelTravel.Services.Data.Models.Home;

namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.InputModels.Home;

    public class HomeController : BaseController
    {
        private readonly IActivitiesService activitiesService;
        private readonly IRestaurantsService restaurantsService;
        private readonly IDestinationsService destinationsService;

        public HomeController(IActivitiesService activitiesService, IRestaurantsService restaurantsService, IDestinationsService destinationsService)
        {
            this.activitiesService = activitiesService;
            this.restaurantsService = restaurantsService;
            this.destinationsService = destinationsService;
        }

        public IActionResult Index()
        {
            this.ViewData["Destinations"] = this.SelectAllDestinations();
            return this.View();
        }

        [HttpPost]
        public IActionResult Search(SearchInputModel searchInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Index");
            }

            var activities = this.activitiesService.GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .Where(a =>
                    a.Location.Destination.Name == searchInputModel.DestinationName &&
                    a.Date >= searchInputModel.StartDate &&
                    a.Date <= searchInputModel.EndDate)
                .ToArray();

            var restaurants = this.restaurantsService.GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .Where(r => r.DestinationName == searchInputModel.DestinationName)
                .ToArray();

            var searchResultViewModel = new SearchResultViewModel
            {
                Activities = activities,
                Restaurants = restaurants,
            };

            return this.View("SearchResult", searchResultViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();

        private IEnumerable<SelectListItem> SelectAllDestinations()
        {
            return this.destinationsService.GetAllDestinationsAsync()
                .GetAwaiter()
                .GetResult()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                });
        }
    }
}

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
        private readonly IDestinationsService destinationsService;

        public HomeController(IDestinationsService destinationsService)
        {
            this.destinationsService = destinationsService;
        }

        public IActionResult Index()
        {
            this.ViewData["Destinations"] = this.SelectAllDestinations();
            return this.View();
        }

        public IActionResult Search(SearchInputModel searchInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Index", searchInputModel);
            }

            var searchResultViewModel = this.destinationsService.GetSearchResult(searchInputModel.DestinationId, searchInputModel.StartDate, searchInputModel.EndDate);

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

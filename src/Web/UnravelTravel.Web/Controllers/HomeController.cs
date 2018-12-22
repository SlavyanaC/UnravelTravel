namespace UnravelTravel.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.Home;
    using UnravelTravel.Services.Data.Contracts;

    public class HomeController : BaseController
    {
        private readonly IDestinationsService destinationsService;

        public HomeController(IDestinationsService destinationsService)
        {
            this.destinationsService = destinationsService;
        }

        public IActionResult Index(SearchInputModel searchInputModel)
        {
            this.ViewData["Destinations"] = this.SelectAllDestinations();

            return searchInputModel.DestinationId != 0 ?
                this.Search(searchInputModel) :
                this.View();
        }

        // TODO: Delete this if private method Search works
        // public IActionResult Search(SearchInputModel searchInputModel)
        // {
        //     if (!this.ModelState.IsValid)
        //     {
        //         return this.RedirectToAction("Index", searchInputModel);
        //     }
        //     var searchResultViewModel = this.destinationsService.GetSearchResult(searchInputModel.DestinationId,  searchInputModel.StartDate, searchInputModel.EndDate);
        //     return this.View("SearchResult", searchResultViewModel);
        // }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();

        private IActionResult Search(SearchInputModel searchInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(searchInputModel);
            }

            var searchResultViewModel = this.destinationsService.GetSearchResult(
                searchInputModel.DestinationId,
                searchInputModel.StartDate,
                searchInputModel.EndDate)
                .GetAwaiter()
                .GetResult();

            return this.View("IndexWithSearchResult", searchResultViewModel);
        }

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

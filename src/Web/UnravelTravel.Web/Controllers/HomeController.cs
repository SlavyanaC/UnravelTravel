namespace UnravelTravel.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Http;
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
            this.HttpContext.Session.SetString("SessionsTest", "Test");
            this.ViewData["Destinations"] = this.SelectAllDestinations();

            return searchInputModel.DestinationId != 0 ?
                this.Search(searchInputModel) :
                this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();

        private IActionResult Search(SearchInputModel searchInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(searchInputModel);
            }

            var searchResultViewModel = this.destinationsService.GetSearchResultAsync(
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

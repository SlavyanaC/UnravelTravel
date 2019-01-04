namespace UnravelTravel.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.Home;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;

    public class HomeController : BaseController
    {
        private readonly IDestinationsService destinationsService;

        public HomeController(IDestinationsService destinationsService)
        {
            this.destinationsService = destinationsService;
        }

        public async Task<IActionResult> Index(SearchInputModel searchInputModel)
        {
            this.HttpContext.Session.SetString("SessionsTest", "Test");
            this.ViewData["Destinations"] = SelectListGenerator.GetAllDestinations(this.destinationsService);

            return searchInputModel.DestinationId != 0 ?
               await this.Search(searchInputModel) :
                this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();

        private async Task<IActionResult> Search(SearchInputModel searchInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(searchInputModel);
            }

            var searchResultViewModel = await this.destinationsService.GetSearchResultAsync(
                searchInputModel.DestinationId,
                searchInputModel.StartDate,
                searchInputModel.EndDate);

            return this.View("IndexWithSearchResult", searchResultViewModel);
        }
    }
}

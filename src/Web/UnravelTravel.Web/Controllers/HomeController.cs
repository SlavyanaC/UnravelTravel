namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
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
            return this.View();
        }

        [Route("[action]")]
        public async Task<IActionResult> Search()
        {
            var destinationId = int.Parse(this.Request.Query["DestinationId"]);
            var startDate = DateTime.Parse(this.Request.Query["StartDate"]);
            var endDate = DateTime.Parse(this.Request.Query["EndDate"]);

            var searchResultViewModel = await this.destinationsService.GetSearchResultAsync(
                destinationId,
                startDate,
                endDate);

            return this.PartialView("_SearchResultPartial", searchResultViewModel);
        }

        public IActionResult Privacy() => this.View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => this.View();
    }
}

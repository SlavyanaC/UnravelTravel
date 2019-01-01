namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Locations;
    using UnravelTravel.Services.Data.Contracts;

    public class LocationsController : AdministratorController
    {
        private readonly ILocationsService locationsService;
        private readonly IDestinationsService destinationsService;

        public LocationsController(ILocationsService locationsService, IDestinationsService destinationsService)
        {
            this.locationsService = locationsService;
            this.destinationsService = destinationsService;
        }

        public IActionResult Create()
        {
            this.ViewData["Destinations"] = this.SelectAllDestinations();
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(LocationCreateInputModel locationCreateInputModel)
        {
            var locationViewModel = await this.locationsService.CreateLocationAsync(locationCreateInputModel);
            return this.RedirectToAction("All");
        }

        public async Task<IActionResult> All()
        {
            var allLocationsViewModel = await this.locationsService.GetAllLocationsAsync();
            return this.View(allLocationsViewModel);
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

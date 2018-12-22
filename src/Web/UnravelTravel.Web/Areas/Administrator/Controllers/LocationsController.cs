namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Locations;
    using UnravelTravel.Services.Data.Contracts;

    public class LocationsController : AdministratorController
    {
        private ILocationsService locationsService;
        private IDestinationsService destinationsService;

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
        public IActionResult Create(LocationCreateInputModel locationCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(locationCreateInputModel);
            }

            var locationId = this.locationsService.CreateAsync(
                    locationCreateInputModel.Name,
                    locationCreateInputModel.Address,
                    locationCreateInputModel.DestinationId,
                    locationCreateInputModel.Type)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("All");
        }

        public IActionResult All()
        {
            var allLocationsViewModel = this.locationsService.GetAllLocationsAsync()
                .GetAwaiter()
                .GetResult();

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

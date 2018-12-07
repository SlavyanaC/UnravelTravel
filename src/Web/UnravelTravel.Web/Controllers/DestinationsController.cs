namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Destinations;

    public class DestinationsController : BaseController
    {
        private readonly IDestinationsService destinationsService;

        public DestinationsController(IDestinationsService destinationsService)
        {
            this.destinationsService = destinationsService;
        }

        public IActionResult All()
        {
            var destinations = this.destinationsService.GetAllDestinationsAsync()
                .GetAwaiter()
                .GetResult();
            if (destinations == null)
            {
                return this.Redirect("/");
            }

            return this.View(destinations);
        }

        public IActionResult Details(int id)
        {
            var destination = this.destinationsService.GetViewModelAsync<DestinationDetailsViewModel>(id)
                .GetAwaiter()
                .GetResult();
            if (destination == null)
            {
                return this.Redirect("/");
            }

            return this.View(destination);
        }
    }
}

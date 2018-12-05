namespace UnravelTravel.Web.Controllers
{
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Destinations;
    using UnravelTravel.Web.InputModels.Destinations;

    public class DestinationsController : BaseController
    {
        private readonly IDestinationsService destinationsService;
        private readonly ICountriesService countriesService;

        public DestinationsController(IDestinationsService destinationsService, ICountriesService countriesService)
        {
            this.destinationsService = destinationsService;
            this.countriesService = countriesService;
        }

        // TODO: Move to admin area
        [Authorize]
        public IActionResult Create()
        {
            this.ViewData["Countries"] = this.countriesService.GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                });

            return this.View();
        }

        // TODO: Move to admin area
        [Authorize]
        [HttpPost]
        public IActionResult Create(DestinationCreateInputModel destinationCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(destinationCreateInputModel);
            }

            var destinationId = this.destinationsService.CreateAsync(
                                destinationCreateInputModel.Name,
                                destinationCreateInputModel.CountryId,
                                destinationCreateInputModel.ImageUrl,
                                destinationCreateInputModel.Information)
                                .GetAwaiter()
                                .GetResult();

            return this.Redirect("/");

            // TODO: Find out why it doesn't work
            // return this.RedirectToAction(nameof(this.Details), destinationId);
        }

        public IActionResult Details(int id)
        {
            var destination = this.destinationsService.GetDestinationDetailsAsync(id)
                .GetAwaiter()
                .GetResult();
            if (destination == null)
            {
                return this.Redirect("/");
            }

            return this.View(destination);
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

        [Authorize]
        public IActionResult Edit(int id)
        {
            this.ViewData["countries"] = this.countriesService.GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                });

            var destinationToEdit = this.destinationsService.GetDestinationToEditAsync(id)
                .GetAwaiter()
                .GetResult();
            if (destinationToEdit == null)
            {
                return this.Redirect("/");
            }

            return this.View(destinationToEdit);
        }

        // TODO: Move to admin area
        [Authorize]
        [HttpPost]
        public IActionResult Edit(DestinationEditViewModel destinationEditViewModel)
        {
            var id = destinationEditViewModel.Id;

            this.destinationsService.EditDestinationAsync(
                id,
                destinationEditViewModel.Name,
                destinationEditViewModel.CountryId,
                destinationEditViewModel.ImageUrl,
                destinationEditViewModel.Information)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction(nameof(this.All));
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            this.ViewData["countries"] = this.countriesService.GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                });

            var destinationToDelete = this.destinationsService.GetDestinationToEditAsync(id)
                .GetAwaiter()
                .GetResult();
            if (destinationToDelete == null)
            {
                return this.Redirect("/");
            }

            return this.View(destinationToDelete);
        }

        // TODO: Move to admin area
        [Authorize]
        [HttpPost]
        public IActionResult Delete(DestinationEditViewModel destinationEditViewModel)
        {
            var id = destinationEditViewModel.Id;
            this.destinationsService.DeleteDestination(id).GetAwaiter().GetResult();
            return this.RedirectToAction(nameof(this.All));
        }
    }
}

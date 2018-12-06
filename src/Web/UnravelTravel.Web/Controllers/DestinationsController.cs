namespace UnravelTravel.Web.Controllers
{
    using System.Collections.Generic;
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
            this.ViewData["Countries"] = this.SelectAllCounties();
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

            return this.RedirectToAction(nameof(this.Details), new { id =destinationId});
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
            this.ViewData["countries"] = this.SelectAllCounties();

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

        // TODO: Move to admin area
        [Authorize]
        public IActionResult Delete(int id)
        {
            this.ViewData["countries"] = this.SelectAllCounties();

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

        private IEnumerable<SelectListItem> SelectAllCounties()
        {
            return this.countriesService.GetAllAsync()
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

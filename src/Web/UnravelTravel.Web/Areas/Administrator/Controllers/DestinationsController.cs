namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Destinations;
    using UnravelTravel.Web.Areas.Administrator.InputModels.Destinations;

    public class DestinationsController : AdministratorController
    {
        private readonly IDestinationsService destinationsService;
        private readonly ICountriesService countriesService;

        public DestinationsController(IDestinationsService destinationsService, ICountriesService countriesService)
        {
            this.destinationsService = destinationsService;
            this.countriesService = countriesService;
        }

        public IActionResult Create()
        {
            this.ViewData["Countries"] = this.SelectAllCounties();
            return this.View();
        }

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

            return this.RedirectToAction("Details", "Destinations", new { area = "", id = destinationId });
        }

        public IActionResult Edit(int id)
        {
            this.ViewData["countries"] = this.SelectAllCounties();

            var destinationToEdit = this.destinationsService.GetViewModelAsync<DestinationEditViewModel>(id)
                .GetAwaiter()
                .GetResult();
            if (destinationToEdit == null)
            {
                return this.Redirect("/");
            }

            return this.View(destinationToEdit);
        }

        [HttpPost]
        public IActionResult Edit(DestinationEditViewModel destinationEditViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(destinationEditViewModel);
            }

            var id = destinationEditViewModel.Id;

            this.destinationsService.EditAsync(
                id,
                destinationEditViewModel.Name,
                destinationEditViewModel.CountryId,
                destinationEditViewModel.ImageUrl,
                destinationEditViewModel.Information)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("All", "Destinations", new { area = "" });
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var destinationToDelete = this.destinationsService.GetViewModelAsync<DestinationEditViewModel>(id)
                .GetAwaiter()
                .GetResult();
            if (destinationToDelete == null)
            {
                return this.Redirect("/");
            }

            return this.View(destinationToDelete);
        }

        [HttpPost]
        public IActionResult Delete(DestinationEditViewModel destinationEditViewModel)
        {
            var id = destinationEditViewModel.Id;
            this.destinationsService.DeleteAsync(id).GetAwaiter().GetResult();
            return this.RedirectToAction("All", "Destinations", new { area = "" });
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

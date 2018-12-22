namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Utilities;

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

            var fileType = destinationCreateInputModel.Image.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(destinationCreateInputModel);
            }

            var destinationId = this.destinationsService.CreateAsync(destinationCreateInputModel)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("Details", "Destinations", new { area = "", id = destinationId });
        }

        public IActionResult Edit(int id)
        {
            this.ViewData["countries"] = this.SelectAllCounties();

            var destinationToEdit = this.destinationsService.GetViewModelByIdAsync<DestinationEditViewModel>(id)
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

            if (destinationEditViewModel.NewImage != null)
            {
                var fileType = destinationEditViewModel.NewImage.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(destinationEditViewModel);
                }
            }

            this.destinationsService.EditAsync(destinationEditViewModel).GetAwaiter().GetResult();

            return this.RedirectToAction("All", "Destinations", new { area = "" });
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var destinationToDelete = this.destinationsService.GetViewModelByIdAsync<DestinationEditViewModel>(id)
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
            this.destinationsService.DeleteByIdAsync(id).GetAwaiter().GetResult();
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

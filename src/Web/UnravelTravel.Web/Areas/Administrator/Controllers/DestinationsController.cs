namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;
    using UnravelTravel.Web.Filters;

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
            this.ViewData["Countries"] = SelectListGenerator.GetAllCountries(this.countriesService);
            return this.View();
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(DestinationCreateInputModel destinationCreateInputModel)
        {
            var fileType = destinationCreateInputModel.Image.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(destinationCreateInputModel);
            }

            var destination = await this.destinationsService.CreateAsync(destinationCreateInputModel);
            return this.RedirectToAction("Details", "Destinations", new { area = "", id = destination.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            this.ViewData["countries"] = SelectListGenerator.GetAllCountries(this.countriesService);
            var destinationToEdit = await this.destinationsService.GetViewModelByIdAsync<DestinationEditViewModel>(id);
            return this.View(destinationToEdit);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Edit(DestinationEditViewModel destinationEditViewModel)
        {
            if (destinationEditViewModel.NewImage != null)
            {
                var fileType = destinationEditViewModel.NewImage.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(destinationEditViewModel);
                }
            }

            await this.destinationsService.EditAsync(destinationEditViewModel);
            return this.RedirectToAction("Details", "Destinations", new { area = "", id= destinationEditViewModel .Id});
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var destinationToDelete = await this.destinationsService.GetViewModelByIdAsync<DestinationDeleteViewModel>(id);
            return this.View(destinationToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DestinationDeleteViewModel destinationEditViewModel)
        {
            var id = destinationEditViewModel.Id;
            await this.destinationsService.DeleteByIdAsync(id);
            return this.RedirectToAction("Index", "Destinations", new { area = "" });
        }
    }
}

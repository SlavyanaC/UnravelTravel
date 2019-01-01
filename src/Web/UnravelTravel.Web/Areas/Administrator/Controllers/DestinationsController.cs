namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Services.Data.Contracts;

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
            this.ViewData["countries"] = this.SelectAllCounties();
            var destinationToEdit = await this.destinationsService.GetViewModelByIdAsync<DestinationEditViewModel>(id);
            return this.View(destinationToEdit);
        }

        [HttpPost]
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
            return this.RedirectToAction("All", "Destinations", new { area = "" });
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var destinationToDelete = await this.destinationsService.GetViewModelByIdAsync<DestinationEditViewModel>(id);
            return this.View(destinationToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DestinationEditViewModel destinationEditViewModel)
        {
            var id = destinationEditViewModel.Id;
            await this.destinationsService.DeleteByIdAsync(id);
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

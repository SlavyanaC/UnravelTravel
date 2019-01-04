namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Activities;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;

    public class ActivitiesController : AdministratorController
    {
        private readonly IActivitiesService activitiesService;
        private readonly IDestinationsService destinationsService;

        public ActivitiesController(IActivitiesService activitiesService, IDestinationsService destinationsService)
        {
            this.activitiesService = activitiesService;
            this.destinationsService = destinationsService;
        }

        public IActionResult Create()
        {
            this.ViewData["Destinations"] = SelectListGenerator.GetAllDestinations(this.destinationsService);
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ActivityCreateInputModel activityCreateInputModel)
        {
            var fileType = activityCreateInputModel.Image.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(activityCreateInputModel);
            }

            var activity = await this.activitiesService.CreateAsync(activityCreateInputModel);
            return this.RedirectToAction("Details", "Activities", new { area = "", id = activity.Id });
        }

        public async Task<IActionResult> Edit(int id)
        {
            this.ViewData["Destinations"] = SelectListGenerator.GetAllDestinations(this.destinationsService);
            var activityToEdit = await this.activitiesService.GetViewModelByIdAsync<ActivityEditViewModel>(id);
            return this.View(activityToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ActivityEditViewModel activityToEditViewModel)
        {
            if (activityToEditViewModel.NewImage != null)
            {
                var fileType = activityToEditViewModel.NewImage.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(activityToEditViewModel);
                }
            }

            await this.activitiesService.EditAsync(activityToEditViewModel);
            return this.RedirectToAction("All", "Activities", new { area = "" });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var activityToDelete = await this.activitiesService.GetViewModelByIdAsync<ActivityDeleteViewModel>(id);
            return this.View(activityToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ActivityDeleteViewModel activityDeleteViewModel)
        {
            var id = activityDeleteViewModel.Id;

            await this.activitiesService.DeleteByIdAsync(id);
            return this.RedirectToAction("All", "Activities", new { area = "" });
        }
    }
}

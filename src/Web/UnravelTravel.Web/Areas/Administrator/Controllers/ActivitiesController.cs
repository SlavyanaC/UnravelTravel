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

    public class ActivitiesController : AdministratorController
    {
        private readonly IActivitiesService activitiesService;
        private readonly ILocationsService locationsService;

        public ActivitiesController(IActivitiesService activitiesService, ILocationsService locationsService)
        {
            this.activitiesService = activitiesService;
            this.locationsService = locationsService;
        }

        public IActionResult Create()
        {
            this.ViewData["Locations"] = this.SelectAllLocations();
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ActivityCreateInputModel activityCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(activityCreateInputModel);
            }

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
            this.ViewData["Locations"] = this.SelectAllLocations();
            var activityToEdit = await this.activitiesService.GetViewModelByIdAsync<ActivityToEditViewModel>(id);
            return this.View(activityToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ActivityToEditViewModel activityToEditViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(activityToEditViewModel);
            }

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
            var activityToDelete = await this.activitiesService.GetViewModelByIdAsync<ActivityToEditViewModel>(id);
            return this.View(activityToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ActivityToEditViewModel activityToEditViewModel)
        {
            var id = activityToEditViewModel.Id;
            await this.activitiesService.DeleteByIdAsync(id);
            return this.RedirectToAction("All", "Activities", new { area = "" });
        }

        private IEnumerable<SelectListItem> SelectAllLocations()
        {
            return this.locationsService.GetAllLocationsAsync()
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

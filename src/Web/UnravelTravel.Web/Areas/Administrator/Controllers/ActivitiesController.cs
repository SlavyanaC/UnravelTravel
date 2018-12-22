namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Activities;
    using UnravelTravel.Web.Areas.Administrator.InputModels.Activities;

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
        public IActionResult Create(ActivityCreateInputModel createInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(createInputModel);
            }

            var fileType = createInputModel.Image.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(createInputModel);
            }

            var activityId = this.activitiesService.CreateAsync(
                createInputModel.Name,
                createInputModel.Image,
                createInputModel.Date,
                createInputModel.Type,
                createInputModel.LocationId,
                createInputModel.Price)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("Details", "Activities", new { area = "", id = activityId });
        }

        public IActionResult Edit(int id)
        {
            this.ViewData["Locations"] = this.SelectAllLocations();
            var activityToEdit = this.activitiesService.GetViewModelAsync<ActivityToEditViewModel>(id)
                .GetAwaiter()
                .GetResult();
            if (activityToEdit == null)
            {
                return this.Redirect("/");
            }

            return this.View(activityToEdit);
        }

        [HttpPost]
        public IActionResult Edit(ActivityToEditViewModel activityToEditViewModel)
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

            var id = activityToEditViewModel.Id;
            this.activitiesService.EditAsync(
                id,
                activityToEditViewModel.Name,
                activityToEditViewModel.NewImage,
                activityToEditViewModel.Date,
                activityToEditViewModel.Type,
                activityToEditViewModel.LocationId)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("All", "Activities", new { area = "" });
        }

        public IActionResult Delete(int id)
        {
            var activityToDelete = this.activitiesService.GetViewModelAsync<ActivityToEditViewModel>(id)
                .GetAwaiter()
                .GetResult();
            if (activityToDelete == null)
            {
                return this.Redirect("/");
            }

            return this.View(activityToDelete);
        }

        [HttpPost]
        public IActionResult Delete(ActivityToEditViewModel activityToEditViewModel)
        {
            var id = activityToEditViewModel.Id;
            this.activitiesService.DeleteAsync(id)
                .GetAwaiter()
                .GetResult();
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

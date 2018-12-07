namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Activities;
    using UnravelTravel.Web.Areas.Administrator.InputModels.Activities;

    public class ActivitiesController : AdministratorController
    {
        private readonly IActivitiesService activitiesService;

        public ActivitiesController(IActivitiesService activitiesService)
        {
            this.activitiesService = activitiesService;
        }

        public IActionResult Create() => this.View();

        [HttpPost]
        public IActionResult Create(ActivityCreateInputModel createInputModel)
        {
            var activityId = this.activitiesService.CreateAsync(
                createInputModel.Name,
                createInputModel.Date,
                createInputModel.Type,
                createInputModel.Location)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("Details", "Activities", new { area = "", id = activityId });
        }

        public IActionResult Edit(int id)
        {
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

            var id = activityToEditViewModel.Id;
            this.activitiesService.EditAsync(
                id,
                activityToEditViewModel.Name,
                activityToEditViewModel.Type,
                activityToEditViewModel.Date,
                activityToEditViewModel.LocationId,
                activityToEditViewModel.LocationName)
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
    }
}

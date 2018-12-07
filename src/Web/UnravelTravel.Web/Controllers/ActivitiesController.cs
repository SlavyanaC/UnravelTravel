namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Activities;
    using UnravelTravel.Web.InputModels.Activities;

    public class ActivitiesController : BaseController
    {
        private readonly IActivitiesService activitiesService;

        public ActivitiesController(IActivitiesService activitiesService)
        {
            this.activitiesService = activitiesService;
        }

        public IActionResult All()
        {
            var activities = this.activitiesService.GetAllActivitiesAsync()
                .GetAwaiter()
                .GetResult();
            if (activities == null)
            {
                return this.Redirect("/");
            }

            return this.View(activities);
        }

        // TODO: Move to admin area
        [Authorize]
        public IActionResult Create() => this.View();

        // TODO: Move to admin area
        [Authorize]
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

            return this.RedirectToAction(nameof(this.Details), new { id = activityId });
        }

        public IActionResult Details(int id)
        {
            var detailsViewModel = this.activitiesService.GetViewModelAsync<ActivityDetailsViewModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(detailsViewModel);
        }

        // TODO: Move to admin area
        [Authorize]
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

        [Authorize]
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

            return this.RedirectToAction(nameof(this.All));
        }

        // TODO: Move to admin area
        [Authorize]
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

        // TODO: Move to admin area
        [Authorize]
        [HttpPost]
        public IActionResult Delete(ActivityToEditViewModel activityToEditViewModel)
        {
            var id = activityToEditViewModel.Id;
            this.activitiesService.DeleteAsync(id)
                .GetAwaiter()
                .GetResult();
            return this.RedirectToAction(nameof(this.All));
        }
    }
}
namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Services.Data.Contracts;

    public class ActivitiesController : BaseController
    {
        private readonly IActivitiesService activitiesService;

        public ActivitiesController(IActivitiesService activitiesService)
        {
            this.activitiesService = activitiesService;
        }

        public IActionResult All()
        {
            var activities = this.activitiesService.GetAllAsync().GetAwaiter().GetResult();
            if (activities == null)
            {
                return this.Redirect("/");
            }

            return this.View(activities);
        }

        public IActionResult Details(int id)
        {
            var detailsViewModel = this.activitiesService.GetViewModelByIdAsync<ActivityDetailsViewModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(detailsViewModel);
        }

        [Authorize]
        public IActionResult Review(int id)
        {
            var reviewInputModel = this.activitiesService.GetViewModelByIdAsync<ActivityReviewInputModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(reviewInputModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Review(int id, ActivityReviewInputModel activityReviewInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(activityReviewInputModel);
            }

            var username = this.User.Identity.Name;

            this.activitiesService.Review(id, username, activityReviewInputModel).GetAwaiter().GetResult();
            return this.RedirectToAction("Details", new { id = id });
        }
    }
}

namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Models.InputModels.Activities;
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
            var activities = this.activitiesService.GetAllAsync()
                .GetAwaiter()
                .GetResult();
            if (activities == null)
            {
                return this.Redirect("/");
            }

            return this.View(activities);
        }

        public IActionResult Details(int id)
        {
            var detailsViewModel = this.activitiesService.GetViewModelAsync<ActivityDetailsViewModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(detailsViewModel);
        }

        [Authorize]
        public IActionResult Review(int id)
        {
            var reviewInputModel = this.activitiesService.GetViewModelAsync<ActivityReviewInputModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(reviewInputModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Review(int id, ActivityReviewInputModel addReviewInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(addReviewInputModel);
            }

            var username = this.User.Identity.Name;

            this.activitiesService.Review(id, username, addReviewInputModel.Rating, addReviewInputModel.Content)
                .GetAwaiter()
                .GetResult();

            return this.RedirectToAction("Details", new { id = id });
        }
    }
}

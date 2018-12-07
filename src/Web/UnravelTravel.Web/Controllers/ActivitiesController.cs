namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Activities;

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

        public IActionResult Details(int id)
        {
            var detailsViewModel = this.activitiesService.GetViewModelAsync<ActivityDetailsViewModel>(id)
                .GetAwaiter()
                .GetResult();
            return this.View(detailsViewModel);
        }
    }
}

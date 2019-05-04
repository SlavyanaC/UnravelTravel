namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;
    using UnravelTravel.Web.Filters;
    using X.PagedList;

    public class ActivitiesController : BaseController
    {
        private readonly IActivitiesService activitiesService;
        private readonly IDestinationsService destinationsService;

        // private readonly IMemoryCache memoryCache;
        public ActivitiesController(IActivitiesService activitiesService, IDestinationsService destinationsService/*, IMemoryCache memoryCache*/)
        {
            this.activitiesService = activitiesService;
            this.destinationsService = destinationsService;

            // this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(ActivityIndexViewModel activityIndexViewModel)
        {
            var activities = await this.activitiesService.GetAllAsync();
            if (activityIndexViewModel.SearchString != null)
            {
                activities = this.activitiesService.GetActivitiesFromSearch(activityIndexViewModel.SearchString, null).ToArray();
            }

            activities = this.activitiesService.SortBy(activities.ToArray(), activityIndexViewModel.Sorter).ToArray();

            var pageNumber = activityIndexViewModel.PageNumber ?? ModelConstants.DefaultPageNumber;
            var pageSize = activityIndexViewModel.PageSize ?? ModelConstants.DefaultPageSize;
            var pageDestinationsViewModel = activities.ToPagedList(pageNumber, pageSize);

            activityIndexViewModel.ActivityViewModels = pageDestinationsViewModel;

            return this.View(activityIndexViewModel);
        }

        public async Task<IActionResult> IndexInDestination(ActivityIndexViewModel activityIndexViewModel, int destinationId)
        {
            var activities = await this.activitiesService.GetAllInDestinationAsync(activityIndexViewModel.DestinationId ?? destinationId);
            if (activityIndexViewModel.SearchString != null)
            {
                activities = this.activitiesService.GetActivitiesFromSearch(activityIndexViewModel.SearchString, activityIndexViewModel.DestinationId).ToArray();
            }

            activities = this.activitiesService.SortBy(activities.ToArray(), activityIndexViewModel.Sorter).ToArray();

            var pageNumber = activityIndexViewModel.PageNumber ?? ModelConstants.DefaultPageNumber;
            var pageSize = activityIndexViewModel.PageSize ?? ModelConstants.DefaultPageSize;
            var pageDestinationsViewModel = activities.ToPagedList(pageNumber, pageSize);

            activityIndexViewModel.DestinationName = await this.destinationsService.GetDestinationName(activityIndexViewModel.DestinationId.Value);
            activityIndexViewModel.ActivityViewModels = pageDestinationsViewModel;
            return this.View(activityIndexViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var detailsViewModel = await this.activitiesService.GetViewModelByIdAsync<ActivityDetailsViewModel>(id);
            return this.View(detailsViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Review(int id)
        {
            var reviewInputModel = await this.activitiesService.GetViewModelByIdAsync<ActivityReviewInputModel>(id);
            return this.View(reviewInputModel);
        }

        [Authorize]
        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Review(int id, ActivityReviewInputModel activityReviewInputModel)
        {
            var username = this.User.Identity.Name;
            await this.activitiesService.Review(id, username, activityReviewInputModel);
            return this.RedirectToAction(nameof(this.Details), new { id });
        }
    }
}

namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;

    public class ActivitiesController : BaseController
    {
        private readonly IActivitiesService activitiesService;
        private readonly IMemoryCache memoryCache;

        public ActivitiesController(IActivitiesService activitiesService, IMemoryCache memoryCache)
        {
            this.activitiesService = activitiesService;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> All()
        {
            if (!this.memoryCache.TryGetValue(WebConstants.AllActivitiesCacheKey, out ActivityViewModel[] cacheEntry))
            {
                cacheEntry = await this.activitiesService.GetAllAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(WebConstants.AllViewMinutesExpiration));
                this.memoryCache.Set(WebConstants.AllActivitiesCacheKey, cacheEntry, cacheEntryOptions);
            }

            return this.View(cacheEntry);
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
        public async Task<IActionResult> Review(int id, ActivityReviewInputModel activityReviewInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(activityReviewInputModel);
            }

            var username = this.User.Identity.Name;
            await this.activitiesService.Review(id, username, activityReviewInputModel);
            return this.RedirectToAction("Details", new { id = id });
        }
    }
}

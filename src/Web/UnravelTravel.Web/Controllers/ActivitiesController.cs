namespace UnravelTravel.Web.Controllers
{
    using System;

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

        public IActionResult All()
        {
            if (!this.memoryCache.TryGetValue(WebConstants.AllActivitiesCacheKey, out ActivityViewModel[] cacheEntry))
            {
                cacheEntry = this.activitiesService.GetAllAsync().GetAwaiter().GetResult();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(WebConstants.AllViewMinutesExpiration));
                this.memoryCache.Set(WebConstants.AllActivitiesCacheKey, cacheEntry, cacheEntryOptions);
            }

            return this.View(cacheEntry);
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

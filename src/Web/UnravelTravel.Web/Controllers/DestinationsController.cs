namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;

    public class DestinationsController : BaseController
    {
        private readonly IDestinationsService destinationsService;
        private readonly IMemoryCache memoryCache;

        public DestinationsController(IDestinationsService destinationsService, IMemoryCache memoryCache)
        {
            this.destinationsService = destinationsService;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> All()
        {
            if (!this.memoryCache.TryGetValue(WebConstants.AllDestinationsCacheKey, out DestinationViewModel[] cacheEntry))
            {
                cacheEntry = await this.destinationsService.GetAllDestinationsAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(WebConstants.AllViewMinutesExpiration));
                this.memoryCache.Set(WebConstants.AllDestinationsCacheKey, cacheEntry, cacheEntryOptions);
            }

            return this.View(cacheEntry);
        }

        public async Task<IActionResult> Details(int id)
        {
            var destination = await this.destinationsService.GetViewModelByIdAsync<DestinationDetailsViewModel>(id);
            return this.View(destination);
        }
    }
}

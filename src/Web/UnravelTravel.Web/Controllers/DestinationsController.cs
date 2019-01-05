namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;
    using X.PagedList;

    public class DestinationsController : BaseController
    {
        private const int DefaultPageNumber = 1;
        private const int DefaultPageSize = 6;

        private readonly IDestinationsService destinationsService;
        private readonly IMemoryCache memoryCache;

        public DestinationsController(IDestinationsService destinationsService, IMemoryCache memoryCache)
        {
            this.destinationsService = destinationsService;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(DestinationIndexViewModel destinationIndexViewModel)
        {
            var destinations = await this.destinationsService.GetAllDestinationsAsync();
            if (destinationIndexViewModel.SearchString != null)
            {
                destinations = this.destinationsService.GetDestinationFromSearch(destinationIndexViewModel.SearchString).ToArray();
            }

            destinations = this.destinationsService.SortBy(destinations, destinationIndexViewModel.Sorter);

            var pageNumber = destinationIndexViewModel.PageNumber ?? DefaultPageNumber;
            var pageSize = destinationIndexViewModel.PageSize ?? DefaultPageSize;
            var pageDestinationsViewModel = destinations.ToPagedList(pageNumber, pageSize);

            destinationIndexViewModel.DestinationViewModels = pageDestinationsViewModel;

            return this.View(destinationIndexViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var destination = await this.destinationsService.GetViewModelByIdAsync<DestinationDetailsViewModel>(id);
            return this.View(destination);
        }
    }
}

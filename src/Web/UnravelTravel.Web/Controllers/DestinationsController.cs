namespace UnravelTravel.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Caching.Memory;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Common;
    using X.PagedList;

    public class DestinationsController : BaseController
    {
        private readonly IDestinationsService destinationsService;

        // private readonly IMemoryCache memoryCache;
        public DestinationsController(IDestinationsService destinationsService/*, IMemoryCache memoryCache*/)
        {
            this.destinationsService = destinationsService;

            // this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(DestinationIndexViewModel destinationIndexViewModel)
        {
            var destinations = await this.destinationsService.GetAllDestinationsAsync();
            if (destinationIndexViewModel.SearchString != null)
            {
                destinations = this.destinationsService.GetDestinationFromSearch(destinationIndexViewModel.SearchString).ToArray();
            }

            destinations = this.destinationsService.SortBy(destinations.ToArray(), destinationIndexViewModel.Sorter);

            var pageNumber = destinationIndexViewModel.PageNumber ?? ModelConstants.DefaultPageNumber;
            var pageSize = destinationIndexViewModel.PageSize ?? ModelConstants.DefaultPageSize;
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

namespace UnravelTravel.Models.ViewModels.Destinations
{
    using UnravelTravel.Models.ViewModels.Enums;
    using X.PagedList;

    public class DestinationIndexViewModel
    {
        public IPagedList<DestinationViewModel> DestinationViewModels { get; set; }

        public string SearchString { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public DestinationSorter Sorter { get; set; }
    }
}

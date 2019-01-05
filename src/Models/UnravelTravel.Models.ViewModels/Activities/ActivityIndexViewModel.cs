namespace UnravelTravel.Models.ViewModels.Activities
{
    using UnravelTravel.Models.ViewModels.Enums;
    using X.PagedList;

    public class ActivityIndexViewModel
    {
        public IPagedList<ActivityViewModel> ActivityViewModels { get; set; }

        public string SearchString { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public int? DestinationId { get; set; }

        public string DestinationName { get; set; }

        public ActivitiesSorter Sorter { get; set; }
    }
}

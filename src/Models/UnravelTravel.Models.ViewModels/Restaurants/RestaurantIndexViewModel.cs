namespace UnravelTravel.Models.ViewModels.Restaurants
{
    using UnravelTravel.Models.ViewModels.Enums;
    using X.PagedList;

    public class RestaurantIndexViewModel
    {
        public IPagedList<RestaurantViewModel> RestaurantViewModels { get; set; }

        public string SearchString { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public RestaurantSorter Sorter { get; set; }
    }
}

namespace UnravelTravel.Models.ViewModels.Home
{
    using System.Collections.Generic;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Models.ViewModels.Restaurants;

    public class SearchResultViewModel
    {
        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public ICollection<ActivityViewModel> Activities { get; set; }

        public ICollection<RestaurantViewModel> Restaurants { get; set; }
    }
}

namespace UnravelTravel.Services.Data.Models.Home
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Services.Data.Models.Activities;
    using UnravelTravel.Services.Data.Models.Restaurants;

    public class SearchResultViewModel
    {
        public string DestinationName { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public ICollection<ActivityViewModel> Activities { get; set; }

        public ICollection<RestaurantViewModel> Restaurants { get; set; }
    }
}

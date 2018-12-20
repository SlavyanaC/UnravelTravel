namespace UnravelTravel.Services.Data.Models.Home
{
    using System.Collections.Generic;

    using UnravelTravel.Services.Data.Models.Activities;
    using UnravelTravel.Services.Data.Models.Restaurants;

    public class SearchResultViewModel
    {
        public ICollection<ActivityViewModel> Activities { get; set; }

        public ICollection<RestaurantViewModel> Restaurants { get; set; }
    }
}

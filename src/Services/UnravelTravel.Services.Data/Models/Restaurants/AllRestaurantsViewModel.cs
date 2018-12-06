namespace UnravelTravel.Services.Data.Models.Restaurants
{
    using System.Collections.Generic;

    public class AllRestaurantsViewModel
    {
        public IEnumerable<RestaurantViewModel> Restaurants { get; set; }
    }
}

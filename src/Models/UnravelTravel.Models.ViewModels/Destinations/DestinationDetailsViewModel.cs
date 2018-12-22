namespace UnravelTravel.Models.ViewModels.Destinations
{
    using System.Collections.Generic;
    using System.Linq;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Mapping;

    public class DestinationDetailsViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public string Information { get; set; }

        public ICollection<Location> Locations { get; set; }

        public ICollection<ActivityViewModel> Activities =>
            this.Locations.SelectMany(l => l.Activities).AsQueryable().To<ActivityViewModel>().ToList();

        public ICollection<ActivityViewModel> TopActivities =>
            this.Activities.OrderByDescending(a => a.AverageRating).Take(3).ToList();

        public int TotalActivities => this.Activities.Count();

        public ICollection<RestaurantViewModel> Restaurants { get; set; }

        public ICollection<RestaurantViewModel> TopRestaurants =>
            this.Restaurants.OrderByDescending(r => r.AverageRating).Take(3).ToList();

        public int TotalRestaurants => this.Restaurants.Count();

        public string MapsAddress => $"{this.Name}+{this.CountryName}";
    }
}

using System;
using UnravelTravel.Models.Common;

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

        public ICollection<ActivityViewModel> Activities { get; set; }

        public ICollection<ActivityViewModel> LatestActivities =>
            this.Activities.Where(a => a.Date >= DateTime.Now).OrderBy(a => a.Date).Take(ModelConstants.DestinationActivitiesToDisplay).ToList();

        public ICollection<RestaurantViewModel> Restaurants { get; set; }

        public ICollection<RestaurantViewModel> TopRestaurants =>
            this.Restaurants.OrderByDescending(r => r.AverageRating).Take(ModelConstants.DestinationRestaurantsToDisplay).ToList();

        public string MapsAddress => $"{this.Name}+{this.CountryName}";
    }
}

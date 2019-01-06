using System.Collections.Generic;
using UnravelTravel.Models.ViewModels.Reviews;

namespace UnravelTravel.Models.ViewModels.Restaurants
{
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class RestaurantDetailsViewModel : IMapFrom<Restaurant>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public int Seats { get; set; }

        public string Type { get; set; }

        public string SplitWordsType => this.Type.SplitWords();

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public double AverageRating { get; set; }

        public string MapsAddress => $"{this.Address}+{this.DestinationName}";

        public IEnumerable<ReviewViewModel> Reviews { get; set; }
    }
}

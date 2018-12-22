namespace UnravelTravel.Models.ViewModels.Restaurants
{
    using System.Collections.Generic;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Reservations;
    using UnravelTravel.Services.Mapping;

    public class RestaurantViewModel : IMapFrom<Restaurant>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string SplitWordsType => this.Type.SplitWords();

        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public int Seats { get; set; }

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public double AverageRating { get; set; }

        public virtual ICollection<ReservationDetailsViewModel> Reservations { get; set; }
    }
}

namespace UnravelTravel.Models.ViewModels.Restaurants
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class RestaurantDeleteViewModel : IMapFrom<Restaurant>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public int Seats { get; set; }

        public string Type { get; set; }

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }
    }
}

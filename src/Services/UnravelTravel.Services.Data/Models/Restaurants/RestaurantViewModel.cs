namespace UnravelTravel.Services.Data.Models.Restaurants
{
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class RestaurantViewModel : IMapFrom<Restaurant>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public int Seats { get; set; }

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }
    }
}

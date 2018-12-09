namespace UnravelTravel.Services.Data.Models.Destinations
{
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class DestinationDetailsViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public string Information { get; set; }

        public int TotalActivities { get; set; }

        public int TotalRestaurants { get; set; }
    }
}

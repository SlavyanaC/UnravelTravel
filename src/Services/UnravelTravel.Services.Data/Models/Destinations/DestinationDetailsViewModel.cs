namespace UnravelTravel.Services.Data.Models.Destinations
{
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class DestinationDetailsViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }

        public string Information { get; set; }

        public int TotalActivities { get; set; }

        public int TotalRestaurants { get; set; }
    }
}

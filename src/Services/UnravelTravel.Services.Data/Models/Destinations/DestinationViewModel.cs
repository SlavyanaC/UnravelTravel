namespace UnravelTravel.Services.Data.Models.Destinations
{
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class DestinationViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public string ImageUrl { get; set; }
    }
}

namespace UnravelTravel.Services.Data.Models.Locations
{
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class LocationViewModel : IMapFrom<Location>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Type { get; set; }

        public string DestinationName { get; set; }
    }
}

namespace UnravelTravel.Models.ViewModels.Destinations
{
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class DestinationDeleteViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public string CountryName { get; set; }

        public string ImageUrl { get; set; }

        public string Information { get; set; }
    }
}

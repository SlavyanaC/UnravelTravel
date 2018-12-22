namespace UnravelTravel.Models.ViewModels.Destinations
{
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class DestinationViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public string ImageUrl { get; set; }
    }
}

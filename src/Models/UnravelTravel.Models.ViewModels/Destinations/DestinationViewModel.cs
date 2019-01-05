namespace UnravelTravel.Models.ViewModels.Destinations
{
    using AutoMapper;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class DestinationViewModel : IHaveCustomMappings, IMapFrom<Destination>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string CountryName { get; set; }

        public string ImageUrl { get; set; }

        public int ActivitiesCount { get; set; }

        public int RestaurantsCount { get; set; }

        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<Destination, DestinationViewModel>()
                .ForMember(d => d.ActivitiesCount, o => o.MapFrom(x => x.Activities.Count))
                .ForMember(d => d.RestaurantsCount, o => o.MapFrom(x => x.Restaurants.Count));
        }
    }
}

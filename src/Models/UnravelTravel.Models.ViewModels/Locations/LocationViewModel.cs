namespace UnravelTravel.Models.ViewModels.Locations
{
    using System.Collections.Generic;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Services.Mapping;

    public class LocationViewModel : IMapFrom<Location>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string LocationType { get; set; }

        public string DestinationName { get; set; }

        public ICollection<ActivityViewModel> Activities { get; set; }
    }
}

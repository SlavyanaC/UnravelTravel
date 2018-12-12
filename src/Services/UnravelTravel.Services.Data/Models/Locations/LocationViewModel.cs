﻿namespace UnravelTravel.Services.Data.Models.Locations
{
    using System.Collections.Generic;

    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Models.Activities;
    using UnravelTravel.Services.Mapping;

    public class LocationViewModel : IMapFrom<Location>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Type { get; set; }

        public string DestinationName { get; set; }

        public ICollection<ActivityViewModel> Activities { get; set; }
    }
}

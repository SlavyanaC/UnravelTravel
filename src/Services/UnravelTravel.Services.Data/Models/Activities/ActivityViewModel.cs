namespace UnravelTravel.Services.Data.Models.Activities
{
    using System;

    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class ActivityViewModel : IMapFrom<Activity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public int LocationId { get; set; }

        public string Location { get; set; }
    }
}

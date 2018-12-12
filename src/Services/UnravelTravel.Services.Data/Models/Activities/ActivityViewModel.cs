namespace UnravelTravel.Services.Data.Models.Activities
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Models.Tickets;
    using UnravelTravel.Services.Mapping;

    public class ActivityViewModel : IMapFrom<Activity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public ICollection<TicketDetailsViewModel> Tickets { get; set; }
    }
}

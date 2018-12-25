namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Tickets;
    using UnravelTravel.Services.Mapping;

    public class ActivityViewModel : IMapFrom<Activity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public string DateAsString => this.Date.ToString(GlobalConstants.DateFormat + " " + GlobalConstants.HourFormat,
            CultureInfo.InvariantCulture);

        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public int LocationDestinationId { get; set; }

        public string LocationDestinationName { get; set; }

        public double AverageRating { get; set; }

        public ICollection<TicketDetailsViewModel> Tickets { get; set; }
    }
}

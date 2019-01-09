namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using UnravelTravel.Common;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Tickets;
    using UnravelTravel.Services.Mapping;

    public class ActivityViewModel : IMapFrom<Activity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; } // UTC

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public double DestinationUtcRawOffset { get; set; }

        public double AverageRating { get; set; }

        public ICollection<TicketDetailsViewModel> Tickets { get; set; }

        public bool HasPassed => this.Date < DateTime.UtcNow;

        //public DateTime LocalDate => this.Date.GetLocalDate(this.DestinationName, this.DestinationCountryName);

        public DateTime LocalDate => this.Date.CalculateLocalDate(this.DestinationUtcRawOffset);

        public string DateAsString => this.LocalDate.ToString(GlobalConstants.DateFormat + " " + GlobalConstants.HourFormat,
            CultureInfo.InvariantCulture);
    }
}

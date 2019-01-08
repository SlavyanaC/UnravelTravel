using GoogleMaps.LocationServices;
using RestSharp;

namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using System.Globalization;
    using System.Collections.Generic;
    using UnravelTravel.Common;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Models.ViewModels.Reviews;

    public class ActivityDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Type { get; set; }

        public string SplitWordsType => this.Type.SplitWords();

        public DateTime Date { get; set; } // UTC from Db

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public string DestinationCountryName { get; set; }

        public string Address { get; set; }

        public string LocationName { get; set; }

        public decimal Price { get; set; }

        public IEnumerable<ReviewViewModel> Reviews { get; set; }

        public bool HasPassed => this.Date < DateTime.UtcNow;

        public DateTime LocalDate => this.Date.GetLocalDate(this.DestinationName, this.DestinationCountryName);

        public string DateString => this.LocalDate.ToString(GlobalConstants.DateFormat + " " + GlobalConstants.HourFormat,
            CultureInfo.InvariantCulture);
    }
}
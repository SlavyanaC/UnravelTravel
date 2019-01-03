using System.Globalization;
using UnravelTravel.Common;

namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using UnravelTravel.Common.Extensions;

    public class ActivityDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Type { get; set; }

        public string SplitWordsType => this.Type.SplitWords();

        public DateTime Date { get; set; }

        public string DateString => this.Date.ToString(GlobalConstants.DateFormat + " " + GlobalConstants.HourFormat,
            CultureInfo.InvariantCulture);

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; }

        public string LocationAddress { get; set; }

        public double AverageRating { get; set; }

        public decimal? Price { get; set; }
    }
}
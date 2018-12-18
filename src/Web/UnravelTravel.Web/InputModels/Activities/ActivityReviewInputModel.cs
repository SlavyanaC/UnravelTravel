namespace UnravelTravel.Web.InputModels.Activities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class ActivityReviewInputModel : IMapFrom<Activity>
    {
        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Content must be between 5 and 500 symbols")]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(1, 5)]
        public double Rating { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string DateAsString => this.Date.ToString(
            GlobalConstants.DateFormat + " " + GlobalConstants.HourFormat,
            CultureInfo.InvariantCulture);
    }
}

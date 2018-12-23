namespace UnravelTravel.Models.InputModels.Reviews
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
        [StringLength(500, MinimumLength = 5, ErrorMessage = InputModelsConstants.ReviewContentError)]
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

namespace UnravelTravel.Models.InputModels.Reviews
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;
    using UnravelTravel.Models.Common;

    public class ActivityReviewInputModel : IMapFrom<Activity>
    {
        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(ModelConstants.Review.ContentMaxLength, MinimumLength = ModelConstants.Review.ContentMinLength, ErrorMessage = ModelConstants.Review.ContentError)]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(ModelConstants.Review.RatingMin, ModelConstants.Review.RatingMax)]
        public double Rating { get; set; } = 1;

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string DateAsString => this.Date.ToString(
            GlobalConstants.DateFormat + " " + GlobalConstants.HourFormat,
            CultureInfo.InvariantCulture);
    }
}

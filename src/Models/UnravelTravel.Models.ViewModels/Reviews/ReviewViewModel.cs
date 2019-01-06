namespace UnravelTravel.Models.ViewModels.Reviews
{
    using System;
    using System.Globalization;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class ReviewViewModel : IMapFrom<RestaurantReview>, IMapFrom<ActivityReview>
    {
        public string ReviewUserUserName { get; set; }

        public double ReviewRating { get; set; }

        public string ReviewContent { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedOnString => this.CreatedOn.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);
    }
}

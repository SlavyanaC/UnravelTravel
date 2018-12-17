namespace UnravelTravel.Services.Data.Models.Tickets
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class TicketDetailsViewModel : IMapFrom<ApplicationUser>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Display(Name = "Full name")]
        public string UserFullName { get; set; }

        public int ActivityId { get; set; }

        public Activity Activity { get; set; }

        [Display(Name = "Activity name")]
        public string ActivityName { get; set; }

        public DateTime ActivityDate { get; set; }

        [Display(Name = "Activity date")]
        public string ActivityDateString => this.ActivityDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = "Activity starting hour")]
        public string ActivityHourString => this.ActivityDate.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);

        [Display(Name = "Location")]
        public string ActivityLocationName { get; set; }

        public bool HasPassed => this.ActivityDate <= DateTime.UtcNow;

        public bool IsRated => this.User.Tickets
            .Any(t => t.Activity.Reviews.Any(ar => ar.ActivityId == this.ActivityId &&
                                                   ar.Review.UserId == this.UserId));
    }
}

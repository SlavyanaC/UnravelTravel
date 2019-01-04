namespace UnravelTravel.Models.ViewModels.Tickets
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class TicketDetailsViewModel : IMapFrom<UnravelTravelUser>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public UnravelTravelUser User { get; set; }

        [Display(Name = ModelConstants.UserFullNameDisplay)]
        public string UserFullName { get; set; }

        public int ActivityId { get; set; }

        public Activity Activity { get; set; }

        [Display(Name = ModelConstants.Activity.NameDisplay)]
        public string ActivityName { get; set; }

        public DateTime ActivityDate { get; set; }

        [Display(Name = ModelConstants.Activity.DateDisplay)]
        public string ActivityDateString => this.ActivityDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = ModelConstants.Activity.StartingHourDisplay)]
        public string ActivityHourString => this.ActivityDate.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);

        [Display(Name = nameof(Destination))]
        public string ActivityDestinationName { get; set; }

        public bool HasPassed => this.ActivityDate <= DateTime.Now;

        public bool IsRated => this.User.Tickets
            .Any(t => t.Activity.Reviews.Any(ar => ar.ActivityId == this.ActivityId &&
                                                   ar.Review.UserId == this.UserId));
    }
}

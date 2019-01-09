namespace UnravelTravel.Models.ViewModels.Tickets
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using UnravelTravel.Common;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class TicketDetailsViewModel : IMapFrom<Ticket>
    {
        [Display(Name = ModelConstants.TicketIdDisplay)]
        public int Id { get; set; }

        public string UserId { get; set; }

        public UnravelTravelUser User { get; set; }

        [Display(Name = ModelConstants.UserFullNameDisplay)]
        public string UserFullName { get; set; }

        public int ActivityId { get; set; }

        public Activity Activity { get; set; }

        [Display(Name = ModelConstants.Activity.NameDisplay)]
        public string ActivityName { get; set; }

        public DateTime ActivityDate { get; set; } // UTC from Db

        public int Quantity { get; set; }

        public string ActivityDestinationId { get; set; }

        [Display(Name = nameof(Destination))]
        public string ActivityDestinationName { get; set; }

        //public string ActivityDestinationCountryName { get; set; }

        public double ActivityDestinationUtcRawOffset { get; set; }

        public bool HasPassed => this.ActivityDate <= DateTime.UtcNow;

        public bool IsRated => this.User.Tickets
            .Any(t => t.Activity.Reviews.Any(ar => ar.ActivityId == this.ActivityId &&
                                                   ar.Review.UserId == this.UserId));

        //public DateTime ActivityLocalDate => this.ActivityDate.GetLocalDate(this.ActivityDestinationName, this.ActivityDestinationCountryName);

        public DateTime ActivityLocalDate => this.ActivityDate.CalculateLocalDate(this.ActivityDestinationUtcRawOffset);


       [Display(Name = ModelConstants.Activity.DateDisplay)]
        public string ActivityDateString => this.ActivityLocalDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = ModelConstants.Activity.StartingHourDisplay)]
        public string ActivityHourString => this.ActivityLocalDate.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);
    }
}

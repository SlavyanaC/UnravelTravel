namespace UnravelTravel.Models.ViewModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class ReservationDetailsViewModel : IMapFrom<Reservation>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Display(Name = "Full name")]
        public string UserFullName { get; set; }

        public int RestaurantId { get; set; }

        [Display(Name = "Restaurant name")]
        public string RestaurantName { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "Reservation date")]
        public string ReservationDateString => this.Date.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = "Reservation hour")]
        public string ReservationHourString => this.Date.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);

        [Display(Name = "Table for")]
        public int PeopleCount { get; set; }

        public bool HasPassed => this.Date < DateTime.UtcNow;

        public bool IsRated => this.User.Reservations
            .Any(t => t.Restaurant.Reviews.Any(rr => rr.RestaurantId == this.RestaurantId &&
                                                     rr.Review.UserId == this.UserId));
    }
}

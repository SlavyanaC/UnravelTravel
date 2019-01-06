namespace UnravelTravel.Models.ViewModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class ReservationDetailsViewModel : IMapFrom<Reservation>
    {
        [Display(Name = ModelConstants.Reservation.IdDisplay)]
        public int Id { get; set; }

        public string UserId { get; set; }

        public UnravelTravelUser User { get; set; }

        [Display(Name = ModelConstants.UserFullNameDisplay)]
        public string UserFullName { get; set; }

        public int RestaurantId { get; set; }

        [Display(Name = ModelConstants.Restaurant.NameDisplay)]
        public string RestaurantName { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = ModelConstants.Reservation.ReservationDay)]
        public string ReservationDayString => this.Date.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = ModelConstants.Reservation.ReservationHour)]
        public string ReservationHourString => this.Date.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);

        [Display(Name = ModelConstants.Reservation.PeopleCountDisplay)]
        public int PeopleCount { get; set; }

        public bool HasPassed => this.Date < DateTime.UtcNow;

        public bool IsRated => this.User.Reservations
            .Any(t => t.Restaurant.Reviews.Any(rr => rr.RestaurantId == this.RestaurantId &&
                                                     rr.Review.UserId == this.UserId));
    }
}

namespace UnravelTravel.Models.InputModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Common.Attributes;
    using UnravelTravel.Models.Common;

    public class ReservationCreateInputModel
    {
        public DateTime Now { get; set; } = DateTime.Now.AddMinutes(ModelConstants.Reservation.MinMinutesFromNow);

        [Required]
        [DataType(DataType.DateTime)]
        [GreaterThan(nameof(Now), ErrorMessage = ModelConstants.Reservation.HourError)]
        [Display(Name = ModelConstants.Reservation.DateDisplay)]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = ModelConstants.Reservation.PeopleCountDisplay)]
        public int PeopleCount { get; set; }
    }
}

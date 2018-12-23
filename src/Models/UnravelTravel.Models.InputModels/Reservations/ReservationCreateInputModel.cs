namespace UnravelTravel.Models.InputModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Common.Attributes;

    public class ReservationCreateInputModel
    {
        public DateTime Now { get; set; } = DateTime.Now.AddMinutes(30);

        [Required]
        [DataType(DataType.DateTime)]
        [GreaterThan(nameof(Now), ErrorMessage = InputModelsConstants.Reservation.HourError)]
        [Display(Name = InputModelsConstants.Reservation.DateDisplay)]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = InputModelsConstants.Reservation.PeopleCountDisplay)]
        public int PeopleCount { get; set; }
    }
}

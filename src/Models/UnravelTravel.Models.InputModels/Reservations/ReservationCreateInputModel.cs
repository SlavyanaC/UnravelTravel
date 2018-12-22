namespace UnravelTravel.Models.InputModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Common.Attributes;

    public class ReservationCreateInputModel
    {
        public DateTime Time { get; set; } = DateTime.Now;

        [Required]
        [DataType(DataType.DateTime)]
        [GreaterThan("Time", ErrorMessage = "Reservation time must be at least 30 minutes from now")]
        [Display(Name = "Reservation date and time")]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Table for...")]
        public int PeopleCount { get; set; }
    }
}

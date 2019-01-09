namespace UnravelTravel.Models.InputModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Models.Common;

    public class ReservationCreateInputModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string GuestUserEmail { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = ModelConstants.Reservation.DateDisplay)]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = ModelConstants.Reservation.PeopleCountDisplay)]
        public int PeopleCount { get; set; }
    }
}

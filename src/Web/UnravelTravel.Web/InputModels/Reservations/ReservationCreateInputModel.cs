namespace UnravelTravel.Web.InputModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReservationCreateInputModel
    {
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Reservation date and time")]
        public DateTime Date { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "Table for...")]
        public int PeopleCount { get; set; }
    }
}

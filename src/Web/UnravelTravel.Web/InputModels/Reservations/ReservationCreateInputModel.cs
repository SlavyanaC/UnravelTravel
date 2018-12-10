namespace UnravelTravel.Web.InputModels.Reservations
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReservationCreateInputModel
    {
        // TODO: User FullName in the Reservation Entity
        //[Required]
        //[Display(Name = "Full name")]
        //public string FullName { get; set; }

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

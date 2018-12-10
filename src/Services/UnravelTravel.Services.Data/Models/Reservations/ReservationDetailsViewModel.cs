namespace UnravelTravel.Services.Data.Models.Reservations
{
    using System.ComponentModel.DataAnnotations;

    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class ReservationDetailsViewModel : IMapFrom<Reservation>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Full name")]
        public string UserFullName { get; set; }

        public int RestaurantId { get; set; }

        [Display(Name = "Restaurant name")]
        public string RestaurantName { get; set; }

        [Display(Name = "Reservation date")]
        public string Date { get; set; }

        [Display(Name = "Table for")]
        public int PeopleCount { get; set; }
    }
}

namespace UnravelTravel.Services.Data.Models.Tickets
{
    using System.ComponentModel.DataAnnotations;

    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class TicketDetailsViewModel : IMapFrom<ApplicationUser>
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Full name")]
        public string UserFullName { get; set; }

        public int ActivityId { get; set; }

        [Display(Name = "Activity name")]
        public string ActivityName { get; set; }

        [Display(Name = "Activity date")]
        public string ActivityDate { get; set; }

        [Display(Name = "Location")]
        public string ActivityLocationName { get; set; }
    }
}

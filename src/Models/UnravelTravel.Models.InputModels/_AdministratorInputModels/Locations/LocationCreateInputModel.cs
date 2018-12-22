using System.ComponentModel.DataAnnotations;

namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Locations
{
    public class LocationCreateInputModel
    {
        [Required]
        [RegularExpression("^[A-Z]\\D+[a-z]$")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Location name must be between 3 and 50 symbols")]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Location address must be between 3 and 50 symbols")]
        public string Address { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int DestinationId { get; set; }

        public string DestinationName { get; set; }
    }
}

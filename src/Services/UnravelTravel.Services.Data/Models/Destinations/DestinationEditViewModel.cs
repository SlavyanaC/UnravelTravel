namespace UnravelTravel.Services.Data.Models.Destinations
{
    using System.ComponentModel.DataAnnotations;

    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class DestinationEditViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression("^[A-Z]\\D+[a-z]$", ErrorMessage = "Destination name cannot contain digits")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Destination name must be between 3 and 50 symbols")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Destination name cannot contain digits")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 50 symbols")]
        public string CountryName { get; set; }

        [Required]
        [Display(Name = "Image Url")]
        [StringLength(400, MinimumLength = 3, ErrorMessage = "ImageUrl name must be between 3 and 400 symbols")]
        public string ImageUrl { get; set; }

        [Required]
        [StringLength(550, MinimumLength = 10, ErrorMessage = "Information must be between 10 and 350 symbols")]
        public string Information { get; set; }
    }
}
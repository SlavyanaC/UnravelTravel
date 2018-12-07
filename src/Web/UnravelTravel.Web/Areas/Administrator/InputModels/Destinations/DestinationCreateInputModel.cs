namespace UnravelTravel.Web.Areas.Administrator.InputModels.Destinations
{
    using System.ComponentModel.DataAnnotations;

    public class DestinationCreateInputModel
    {
        [Required]
        [RegularExpression("^[A-Z]\\D+[a-z]$")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Destination name must be between 3 and 50 symbols")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Country")]
        public int CountryId { get; set; }

        [Required]
        [Display(Name="Image Url")]
        [StringLength(400, MinimumLength = 3, ErrorMessage = "ImageUrl name must be between 3 and 400 symbols")]
        public string ImageUrl { get; set; }

        [Required]
        [StringLength(550, MinimumLength = 10, ErrorMessage = "Information must be between 10 and 350 symbols")]
        public string Information { get; set; }
    }
}

namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

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
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(550, MinimumLength = 10, ErrorMessage = "Information must be between 10 and 350 symbols")]
        public string Information { get; set; }
    }
}

// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class DestinationCreateInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = AdminInputModelsConstants.NameError)]
        [RegularExpression(AdminInputModelsConstants.NameRegex, ErrorMessage = AdminInputModelsConstants.NameRegexError)]
        public string Name { get; set; }

        [Required]
        [Display(Name = nameof(Country))]
        public int CountryId { get; set; }

        public string Country { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(550, MinimumLength = 10, ErrorMessage = AdminInputModelsConstants.Destination.InformationError)]
        public string Information { get; set; }
    }
}

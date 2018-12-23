// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Data.Models;

    public class DestinationCreateInputModel
    {
        [Required]
        [StringLength(ModelConstants.Destination.NameMaxLength, MinimumLength = ModelConstants.Destination.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        [RegularExpression(ModelConstants.NameRegex, ErrorMessage = ModelConstants.NameRegexError)]
        public string Name { get; set; }

        [Required]
        [Display(Name = nameof(Country))]
        public int CountryId { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(ModelConstants.Destination.InformationMaxLength, MinimumLength = ModelConstants.Destination.InformationMinLength, ErrorMessage = ModelConstants.Destination.InformationError)]
        public string Information { get; set; }
    }
}

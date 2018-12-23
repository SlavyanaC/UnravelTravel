// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Locations
{
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Models.Common;

    public class LocationCreateInputModel
    {
        [Required]
        [StringLength(ModelConstants.Location.NameMaxLength, MinimumLength = ModelConstants.Location.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        [RegularExpression(ModelConstants.NameRegex, ErrorMessage = ModelConstants.NameRegexError)]
        public string Name { get; set; }

        [Required]
        [StringLength(ModelConstants.AddressMaxLength, MinimumLength = ModelConstants.AddressMinLength, ErrorMessage = ModelConstants.AddressLengthError)]
        public string Address { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int DestinationId { get; set; }
    }
}

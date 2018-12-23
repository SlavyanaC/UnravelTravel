// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Locations
{
    using System.ComponentModel.DataAnnotations;

    public class LocationCreateInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = AdminInputModelsConstants.NameError)]
        [RegularExpression(AdminInputModelsConstants.NameRegex, ErrorMessage = AdminInputModelsConstants.NameRegexError)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = AdminInputModelsConstants.AddressError)]
        public string Address { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public int DestinationId { get; set; }

        public string DestinationName { get; set; }
    }
}

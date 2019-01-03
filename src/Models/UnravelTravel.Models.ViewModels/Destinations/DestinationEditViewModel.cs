namespace UnravelTravel.Models.ViewModels.Destinations
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class DestinationEditViewModel : IMapFrom<Destination>
    {
        public int Id { get; set; }

        [Required]
        [RegularExpression(ModelConstants.NameRegex, ErrorMessage = ModelConstants.NameRegexError)]
        [StringLength(ModelConstants.Destination.NameMaxLength, MinimumLength = ModelConstants.Destination.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        [Display(Name = nameof(Country))]
        public int CountryId { get; set; }

        public string CountryName { get; set; }

        [Display(Name = ModelConstants.ImageUrlDisplay)]
        public string ImageUrl { get; set; }

        [Display(Name = ModelConstants.NewImageDisplay)]
        [DataType(DataType.Upload)]
        public IFormFile NewImage { get; set; }

        [Required]
        [StringLength(ModelConstants.Destination.InformationMaxLength, MinimumLength = ModelConstants.Destination.InformationMinLength, ErrorMessage = ModelConstants.Destination.InformationError)]
        public string Information { get; set; }
    }
}

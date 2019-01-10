// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Activities
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;

    public class ActivityCreateInputModel
    {
        [Required]
        [StringLength(ModelConstants.Activity.NameMaxLength, MinimumLength = ModelConstants.Activity.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        [Required]
        [Display(Name = ModelConstants.Activity.AdminDateDisplay)]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(ModelConstants.Activity.DescriptionMaxLength, MinimumLength = ModelConstants.Activity.DescriptionMinLength, ErrorMessage = ModelConstants.Activity.DescriptionLengthError)]
        public string Description { get; set; }

        [Display(Name = ModelConstants.Activity.AdditionalInfoDisplay)]
        public string AdditionalInfo { get; set; }

        [Required]
        [Display(Name = nameof(Destination))]
        public int DestinationId { get; set; }

        [Required]
        [StringLength(ModelConstants.AddressMaxLength, MinimumLength = ModelConstants.AddressMinLength, ErrorMessage = ModelConstants.AddressLengthError)]
        public string Address { get; set; }
        
        [Display(Name = ModelConstants.Activity.LocationNameDisplay)]
        public string LocationName { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}

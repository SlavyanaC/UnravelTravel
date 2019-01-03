namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using UnravelTravel.Common.Attributes;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class ActivityEditViewModel : IMapFrom<Activity>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ModelConstants.Activity.NameMaxLength, MinimumLength = ModelConstants.Activity.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Display(Name = ModelConstants.ImageUrlDisplay)]
        public string ImageUrl { get; set; }

        [Display(Name = ModelConstants.NewImageDisplay)]
        [DataType(DataType.Upload)]
        public IFormFile NewImage { get; set; }

        [Required]
        public string Type { get; set; }

        public DateTime Now => DateTime.Now.AddMinutes(ModelConstants.Activity.MinMinutesTillStart);

        [Required]
        [Display(Name = ModelConstants.Activity.AdminDateDisplay)]
        [DataType(DataType.DateTime)]
        [GreaterThan(nameof(Now), ErrorMessage = ModelConstants.Activity.StartingHourError)]
        public DateTime Date { get; set; }

        [Required]
        [StringLength(ModelConstants.Activity.DescriptionMaxLength, MinimumLength = ModelConstants.Activity.DescriptionMinLength, ErrorMessage = ModelConstants.Activity.DescriptionLengthError)]
        public string Description { get; set; }

        [Display(Name = ModelConstants.Activity.AdditionalInfoDisplay)]
        public string AdditionalInfo { get; set; }

        [Required]
        public int LocationId { get; set; }

        public string LocationName { get; set; }
    }
}

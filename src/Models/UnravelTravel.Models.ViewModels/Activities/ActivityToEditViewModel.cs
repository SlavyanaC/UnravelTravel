namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class ActivityToEditViewModel : IMapFrom<Activity>
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

        [Required]
        [Display(Name = ModelConstants.Activity.AdminDateDisplay)]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        public int LocationId { get; set; }

        public string LocationName { get; set; }
    }
}

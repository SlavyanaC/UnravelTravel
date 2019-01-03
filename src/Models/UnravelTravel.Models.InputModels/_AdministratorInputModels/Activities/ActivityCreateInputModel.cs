// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Activities
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Common.Attributes;
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

        public DateTime Now => DateTime.Now.AddMinutes(ModelConstants.Activity.MinMinutesTillStart);

        [Required]
        [Display(Name = ModelConstants.Activity.AdminDateDisplay)]
        [DataType(DataType.DateTime)]
        [GreaterThan(nameof(Now), ErrorMessage = ModelConstants.Activity.StartingHourError)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = nameof(Location))]
        public int LocationId { get; set; }

        public decimal Price { get; set; }
    }
}

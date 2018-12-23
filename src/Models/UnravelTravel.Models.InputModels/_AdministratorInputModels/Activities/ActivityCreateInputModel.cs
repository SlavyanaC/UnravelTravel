// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Activities
{
    using Microsoft.AspNetCore.Http;
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Common.Attributes;
    using UnravelTravel.Models.InputModels.AdministratorInputModels;

    public class ActivityCreateInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = AdminInputModelsConstants.NameError)]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public IFormFile Image { get; set; }

        public DateTime Now => DateTime.Now.AddMinutes(15);

        [Required]
        [Display(Name = AdminInputModelsConstants.Activity.DateDisplayName)]
        [DataType(DataType.DateTime)]
        [GreaterThan(nameof(Now), ErrorMessage = AdminInputModelsConstants.Activity.StartingHourError)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = nameof(Location))]
        public int LocationId { get; set; }

        public string Location { get; set; }

        public decimal? Price { get; set; }
    }
}

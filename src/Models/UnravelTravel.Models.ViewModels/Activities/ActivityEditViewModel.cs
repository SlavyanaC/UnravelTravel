﻿using Newtonsoft.Json;
using UnravelTravel.Common.Extensions;

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
        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        [Required]
        [StringLength(ModelConstants.AddressMaxLength, MinimumLength = ModelConstants.AddressMinLength, ErrorMessage = ModelConstants.AddressLengthError)]
        public string Address { get; set; }

        public string LocationName { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}

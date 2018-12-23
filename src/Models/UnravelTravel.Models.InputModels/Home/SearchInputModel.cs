namespace UnravelTravel.Models.InputModels.Home
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Common.Attributes;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;

    public class SearchInputModel
    {
        [Required]
        [Display(Name = nameof(Destination))]
        public int DestinationId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = ModelConstants.Search.StartDateDisplay)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [GreaterThan(nameof(StartDate), ErrorMessage = ModelConstants.Search.EndDateError)]
        [Display(Name = ModelConstants.Search.EndDateDisplay)]
        public DateTime EndDate { get; set; }
    }
}

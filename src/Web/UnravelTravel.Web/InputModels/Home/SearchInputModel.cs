namespace UnravelTravel.Web.InputModels.Home
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using UnravelTravel.Data.Models;

    public class SearchInputModel
    {
        [Required]
        [Display(Name = "Destination")]
        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "From")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "To")]
        public DateTime EndDate { get; set; }
    }
}

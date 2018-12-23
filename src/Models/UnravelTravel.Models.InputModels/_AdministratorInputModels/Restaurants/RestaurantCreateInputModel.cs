// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Data.Models;

    public class RestaurantCreateInputModel
    {
        [Required]
        [StringLength(ModelConstants.Restaurant.NameMaxLength, MinimumLength = ModelConstants.Restaurant.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(ModelConstants.AddressMaxLength, MinimumLength = ModelConstants.AddressMinLength, ErrorMessage = ModelConstants.AddressLengthError)]
        public string Address { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Seats { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Display(Name = nameof(Destination))]
        public int DestinationId { get; set; }
    }
}

namespace UnravelTravel.Models.ViewModels.Restaurants
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class RestaurantEditViewModel : IMapFrom<Restaurant>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(ModelConstants.Restaurant.NameMaxLength, MinimumLength = ModelConstants.Restaurant.NameMinLength, ErrorMessage = ModelConstants.NameLengthError)]
        public string Name { get; set; }

        [Display(Name = ModelConstants.ImageUrlDisplay)]
        public string ImageUrl { get; set; }

        [Display(Name = ModelConstants.NewImageDisplay)]
        [DataType(DataType.Upload)]
        public IFormFile NewImage { get; set; }

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

        public string DestinationName { get; set; }
    }
}

// ReSharper disable once CheckNamespace
namespace UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants
{
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Http;

    public class RestaurantCreateInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = AdminInputModelsConstants.NameError)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Image { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = AdminInputModelsConstants.AddressError)]
        public string Address { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Seats { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Display(Name = nameof(Destination))]
        public int DestinationId { get; set; }

        public string Destination { get; set; }
    }
}

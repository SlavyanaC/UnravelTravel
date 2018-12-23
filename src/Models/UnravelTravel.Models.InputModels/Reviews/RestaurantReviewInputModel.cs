namespace UnravelTravel.Models.InputModels.Reviews
{
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class RestaurantReviewInputModel : IMapFrom<Restaurant>
    {
        [Required]
        [DataType(DataType.Currency)]
        [Range(1, 5)]
        public double Rating { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(500, MinimumLength = 5, ErrorMessage = InputModelsConstants.ReviewContentError)]
        public string Content { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}

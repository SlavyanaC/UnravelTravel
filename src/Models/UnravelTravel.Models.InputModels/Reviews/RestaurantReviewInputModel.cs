namespace UnravelTravel.Models.InputModels.Reviews
{
    using System.ComponentModel.DataAnnotations;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;
    using UnravelTravel.Models.Common;

    public class RestaurantReviewInputModel : IMapFrom<Restaurant>
    {
        [Required]
        [DataType(DataType.Currency)]
        [Range(ModelConstants.Review.RatingMin, ModelConstants.Review.RatingMax)]
        public double Rating { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [StringLength(ModelConstants.Review.ContentMaxLength, MinimumLength = ModelConstants.Review.ContentMinLength, ErrorMessage = ModelConstants.Review.ContentError)]
        public string Content { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}

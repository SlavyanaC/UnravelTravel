namespace UnravelTravel.Services.Data.Models.Activities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class ActivityToEditViewModel : IMapFrom<Activity>
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 symbols")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Image Url")]
        [StringLength(400, MinimumLength = 3, ErrorMessage = "ImageUrl name must be between 3 and 400 symbols")]
        public string ImageUrl { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Date and time of the activity")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        [Required]
        public int LocationId { get; set; }

        public string LocationName { get; set; }
    }
}

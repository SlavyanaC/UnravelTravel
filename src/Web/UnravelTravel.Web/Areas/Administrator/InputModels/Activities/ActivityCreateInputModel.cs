namespace UnravelTravel.Web.Areas.Administrator.InputModels.Activities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ActivityCreateInputModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 symbols")]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        [Display(Name = "Image Url")]
        [StringLength(400, MinimumLength = 3, ErrorMessage = "ImageUrl name must be between 3 and 400 symbols")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Date and time of the activity")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public int LocationId { get; set; }

        [Required]
        public string Location { get; set; }
    }
}

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
        [Display(Name = "Date and time of the activity")]
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }

        public int LocationId { get; set; }

        [Required]
        public string Location { get; set; }
    }
}

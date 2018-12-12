namespace UnravelTravel.Services.Data.Models.Activities
{
    using System;

    public class ActivityDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; }
    }
}

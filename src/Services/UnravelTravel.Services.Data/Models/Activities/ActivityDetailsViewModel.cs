namespace UnravelTravel.Services.Data.Models.Activities
{
    using System;

    public class ActivityDetailsViewModel
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public int LocationId { get; set; }

        public string LocationName { get; set; }
    }
}

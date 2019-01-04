namespace UnravelTravel.Models.ViewModels.Activities
{
    using System;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class ActivityDeleteViewModel : IMapFrom<Activity>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Type { get; set; }

        public DateTime Now => DateTime.Now.AddMinutes(ModelConstants.Activity.MinMinutesTillStart);

        public DateTime Date { get; set; }

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public int DestinationId { get; set; }

        public string DestinationName { get; set; }

        public string LocationName { get; set; }

        public string Address { get; set; }

        public decimal Price { get; set; }
    }
}

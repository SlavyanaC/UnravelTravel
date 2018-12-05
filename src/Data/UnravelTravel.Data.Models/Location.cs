namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Data.Common.Models;
    using UnravelTravel.Data.Models.Enums;

    public class Location : BaseModel<int>, IDeletableEntity
    {
        public Location()
        {
            this.Activities = new HashSet<Activity>();
        }

        public string Name { get; set; }

        public string Address { get; set; }

        public LocationType LocationType { get; set; }

        public int DestinationId { get; set; }

        public virtual Destination Destination { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }
    }
}

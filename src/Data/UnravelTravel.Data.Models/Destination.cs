namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Data.Common.Models;

    public class Destination : BaseModel<int>, IDeletableEntity
    {
        public Destination()
        {
            this.Activities = new HashSet<Activity>();
            this.Restaurants = new HashSet<Restaurant>();
        }

        public string Name { get; set; }

        public string Information { get; set; }

        public string ImageUrl { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Activity> Activities { get; set; }

        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}

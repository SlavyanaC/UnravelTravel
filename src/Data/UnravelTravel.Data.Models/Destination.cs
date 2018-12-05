namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Data.Common.Models;

    public class Destination : BaseModel<int>, IDeletableEntity
    {
        public Destination()
        {
            this.Locations = new HashSet<Location>();
        }

        public string Name { get; set; }

        public string Information { get; set; }

        public string ImageUrl { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Location> Locations { get; set; }
    }
}

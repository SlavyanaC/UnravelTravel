namespace UnravelTravel.Data.Models
{
    using System.Collections.Generic;

    using UnravelTravel.Data.Common.Models;

    public class Country : BaseModel<int>
    {
        public Country()
        {
            this.Destinations = new HashSet<Destination>();
        }

        public string Name { get; set; }

        public virtual ICollection<Destination> Destinations { get; set; }
    }
}

namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Data.Common.Models;
    using UnravelTravel.Data.Models.Enums;

    public class Activity : BaseModel<int>, IDeletableEntity
    {
        public Activity()
        {
            this.Tickets = new HashSet<Ticket>();
            this.ActivityTags = new HashSet<ActivityTag>();
        }

        public string Name { get; set; }

        public ActivityType Type { get; set; }

        public DateTime Date { get; set; }

        public int LocationId { get; set; }

        public virtual Location Location { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        public virtual ICollection<ActivityTag> ActivityTags { get; set; }
    }
}

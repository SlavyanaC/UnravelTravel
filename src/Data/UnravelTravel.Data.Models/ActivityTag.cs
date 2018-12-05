namespace UnravelTravel.Data.Models
{
    using System;

    using UnravelTravel.Data.Common.Models;

    public class ActivityTag : IDeletableEntity
    {
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public int TagId { get; set; }

        public virtual Tag Tag { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

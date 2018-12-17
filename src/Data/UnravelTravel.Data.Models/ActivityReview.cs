namespace UnravelTravel.Data.Models
{
    using System;

    using UnravelTravel.Data.Common.Models;

    public class ActivityReview : BaseModel<int>, IDeletableEntity
    {
        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public int ReviewId { get; set; }

        public virtual Review Review { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

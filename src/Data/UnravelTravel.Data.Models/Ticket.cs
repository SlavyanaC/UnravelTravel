namespace UnravelTravel.Data.Models
{
    using System;

    using UnravelTravel.Data.Common.Models;

    public class Ticket : BaseModel<int>, IDeletableEntity
    {
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

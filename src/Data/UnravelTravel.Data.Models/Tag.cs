namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Data.Common.Models;

    public class Tag : BaseModel<int>, IDeletableEntity
    {
        public Tag()
        {
            this.ActivityTags = new HashSet<ActivityTag>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<ActivityTag> ActivityTags { get; set; }
    }
}

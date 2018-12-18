namespace UnravelTravel.Data.Models
{
    using System;

    using UnravelTravel.Data.Common.Models;

    public class RestaurantReview : BaseModel<int>, IDeletableEntity
    {
        public int RestaurantId { get; set; }

        public virtual Restaurant Restaurant { get; set; }

        public int ReviewId { get; set; }

        public virtual Review Review { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

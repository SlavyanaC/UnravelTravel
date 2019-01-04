namespace UnravelTravel.Data.Models
{
    using System;

    using UnravelTravel.Data.Common.Models;

    public class Reservation : BaseModel<int>, IDeletableEntity
    {
        public string UserId { get; set; }

        public virtual UnravelTravelUser User { get; set; }

        public int RestaurantId { get; set; }

        public virtual Restaurant Restaurant { get; set; }

        public DateTime Date { get; set; }

        public int PeopleCount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

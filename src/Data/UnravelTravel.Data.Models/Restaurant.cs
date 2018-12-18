namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;

    using UnravelTravel.Data.Common.Models;
    using UnravelTravel.Data.Models.Enums;

    public class Restaurant : BaseModel<int>, IDeletableEntity
    {
        public Restaurant()
        {
            this.Reservations = new HashSet<Reservation>();
            this.Reviews = new HashSet<RestaurantReview>();
        }

        public string Name { get; set; }

        public string ImageUrl { get; set; }

        public string Address { get; set; }

        public int Seats { get; set; }

        public RestaurantType Type { get; set; }

        public int DestinationId { get; set; }

        public virtual Destination Destination { get; set; }

        public double AverageRating { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ICollection<RestaurantReview> Reviews { get; set; }
    }
}

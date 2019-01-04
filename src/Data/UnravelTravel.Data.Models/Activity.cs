﻿namespace UnravelTravel.Data.Models
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
            this.Reviews = new HashSet<ActivityReview>();
        }

        public string Name { get; set; }

        public ActivityType Type { get; set; }

        public string ImageUrl { get; set; }

        public DateTime Date { get; set; }

        public decimal Price { get; set; }

        public string Description { get; set; }

        public string AdditionalInfo { get; set; }

        public string Address { get; set; }

        public string LocationName { get; set; }

        public int DestinationId { get; set; }

        public virtual Destination Destination { get; set; }

        public double AverageRating { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

        public virtual ICollection<ActivityReview> Reviews { get; set; }
    }
}

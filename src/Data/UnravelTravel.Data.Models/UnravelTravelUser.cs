﻿// ReSharper disable VirtualMemberCallInConstructor
namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Identity;
    using UnravelTravel.Data.Common.Models;

    public class UnravelTravelUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public UnravelTravelUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();

            this.Tickets = new HashSet<Ticket>();
            this.Reservations = new HashSet<Reservation>();
            this.Reviews = new HashSet<Review>();
        }

        public string FullName { get; set; }

        public int ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        // Audit info
        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        // Deletable entity
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        // My properties
        public virtual ICollection<Ticket> Tickets { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }
    }
}

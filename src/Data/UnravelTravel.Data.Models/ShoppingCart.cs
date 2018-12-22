namespace UnravelTravel.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using UnravelTravel.Data.Common.Models;

    public class ShoppingCart : BaseModel<int>
    {
        public ShoppingCart()
        {
            this.ShoppingCartActivities = new HashSet<ShoppingCartActivity>();
        }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<ShoppingCartActivity> ShoppingCartActivities { get; set; }
    }
}

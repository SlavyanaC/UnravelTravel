namespace UnravelTravel.Data.Models
{
    using System;

    using UnravelTravel.Data.Common.Models;

    public class ShoppingCartActivity : BaseModel<int>, IDeletableEntity
    {
        public int ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        public int ActivityId { get; set; }

        public virtual Activity Activity { get; set; }

        public int Quantity { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}

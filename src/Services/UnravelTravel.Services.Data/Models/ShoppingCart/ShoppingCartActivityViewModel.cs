namespace UnravelTravel.Services.Data.Models.ShoppingCart
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;

    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class ShoppingCartActivityViewModel : IMapFrom<ShoppingCartActivity>
    {
        public int Id { get; set; }

        public string ShoppingCartUserId { get; set; }

        public ApplicationUser ShoppingCartUser { get; set; }

        [Display(Name = "Full name")]

        public string ShoppingCartUserFullName { get; set; }

        public decimal? ActivityPrice { get; set; }

        public int ActivityId { get; set; }

        public Activity Activity { get; set; }

        public string ActivityImageUrl { get; set; }

        [Display(Name = "Activity name")]
        public string ActivityName { get; set; }

        public DateTime ActivityDate { get; set; }

        [Display(Name = "Activity date")]
        public string DateString => this.ActivityDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = "Activity starting hour")]
        public string ActivityHourString => this.ActivityDate.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);

        [Display(Name = "Location")]
        public string ActivityLocationName { get; set; }

        public int Quantity { get; set; }

        public decimal ShoppingCartActivityTotalPrice => this.Activity.Price.Value * this.Quantity;
    }
}

namespace UnravelTravel.Models.ViewModels.ShoppingCart
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class ShoppingCartActivityViewModel : IMapFrom<ShoppingCartActivity>
    {
        public int Id { get; set; }

        public string ShoppingCartUserId { get; set; }

        public int ActivityId { get; set; }

        public string ActivityImageUrl { get; set; }

        [Display(Name = ModelConstants.Activity.NameDisplay)]
        public string ActivityName { get; set; }

        public DateTime ActivityDate { get; set; }

        public decimal? ActivityPrice { get; set; }

        public string DateString => this.ActivityDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = ModelConstants.Activity.StartingHourDisplay)]
        public string ActivityHourString => this.ActivityDate.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);

        [Display(Name = nameof(Location))]
        public string ActivityLocationName { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal? ShoppingCartActivityTotalPrice => this.ActivityPrice * this.Quantity;
    }
}

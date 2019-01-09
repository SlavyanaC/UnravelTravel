﻿namespace UnravelTravel.Models.ViewModels.ShoppingCart
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using UnravelTravel.Common;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.Common;
    using UnravelTravel.Services.Mapping;

    public class ShoppingCartActivityViewModel : IMapFrom<ShoppingCartActivity>
    {
        public int Id { get; set; }

        public string ShoppingCartUserId { get; set; }

        public int ActivityId { get; set; }

        public string ActivityImageUrl { get; set; }

        public string ActivityName { get; set; }

        [Display(Name = ModelConstants.Activity.NameDisplay)]
        public string ActivityNameSubstring => this.ActivityName.Length >= ModelConstants.ShoppingCartActivityNameLength ? this.ActivityName.Substring(0, ModelConstants.ShoppingCartActivityNameLength) + ModelConstants.ShoppingCartActivityNameCut : this.ActivityName;

        public DateTime ActivityDate { get; set; }

        //public DateTime ActivityLocalDate => this.ActivityDate.GetLocalDate(this.ActivityDestinationName, this.ActivityDestinationCountryName);

        public DateTime ActivityLocalDate => this.ActivityDate.CalculateLocalDate(this.ActivityDestinationUtcRawOffset);

        public decimal? ActivityPrice { get; set; }

        public string DateString => this.ActivityLocalDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture);

        [Display(Name = ModelConstants.Activity.StartingHourDisplay)]
        public string ActivityHourString => this.ActivityLocalDate.ToString(GlobalConstants.HourFormat, CultureInfo.InvariantCulture);

        public int ActivityDestinationId { get; set; }

        [Display(Name = nameof(Destination))]
        public string ActivityDestinationName { get; set; }

        public double ActivityDestinationUtcRawOffset { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal? ShoppingCartActivityTotalPrice => this.ActivityPrice * this.Quantity;
    }
}

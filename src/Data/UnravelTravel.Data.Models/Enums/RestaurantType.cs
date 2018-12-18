namespace UnravelTravel.Data.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum RestaurantType
    {
        Bar = 1,
        Barbecue = 2,
        Bistro = 3,
        Buffet = 4,
        Cafe = 5,
        [Display(Name = "Casual Dining")]
        CasualDining = 6,
        Chinese = 7,
        [Display(Name = "Coffee Shop")]
        CoffeeShop = 8,
        [Display(Name = "Drive Thru")]
        DriveThru = 9,
        [Display(Name = "Fast Food")]
        FastFood = 10,
        [Display(Name = "Fine Dining")]
        FineDining = 11,
        [Display(Name = "Food Truck")]
        FoodTruck = 12,
        Italian = 13,
        Pub = 14,
        Traditional = 15,
        Other = 16,
    }
}

namespace UnravelTravel.Data.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum ActivityType
    {
        Adventure = 1,
        Concert = 2,
        Culinary = 3,
        Culture = 4,
        Extreme = 5,
        Hiking = 6,
        Photography = 7,
        Recreation = 8,
        [Display(Name = "Road Trip")]
        RoadTrip = 9,
        [Display(Name = "Self Guided")]
        SelfGuided = 10,
        Shopping = 11,
        Spiritual = 12,
        Sport = 13,
        Wildlife = 14,
        Winter = 15,
        Other = 16,
    }
}

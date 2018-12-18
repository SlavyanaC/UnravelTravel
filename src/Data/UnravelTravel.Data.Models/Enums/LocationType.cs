namespace UnravelTravel.Data.Models.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum LocationType
    {
        [Display(Name = "Amusement Park")]
        AmusementPark = 1,
        Bar = 2,
        Cafe = 3,
        Campground = 4,
        Casino = 5,
        Museum = 6,
        [Display(Name = "Open Space")]
        OpenSpace = 7,
        Park = 8,
        Restaurant = 10,
        Stadium = 11,
        Venue = 12,
        Zoo = 13,
        Other = 14,
    }
}

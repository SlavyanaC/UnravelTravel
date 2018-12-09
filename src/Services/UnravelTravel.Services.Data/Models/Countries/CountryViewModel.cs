namespace UnravelTravel.Services.Data.Models.Countries
{
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Mapping;

    public class CountryViewModel : IMapFrom<Country>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}

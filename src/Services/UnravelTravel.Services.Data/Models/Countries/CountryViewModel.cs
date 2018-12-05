namespace UnravelTravel.Services.Data.Models.Countries
{
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Models;

    public class CountryViewModel : IMapFrom<Country>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}

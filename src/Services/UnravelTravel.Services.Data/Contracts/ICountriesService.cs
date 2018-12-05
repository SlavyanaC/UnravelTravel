namespace UnravelTravel.Services.Data.Contracts
{
    using System.Collections.Generic;

    using UnravelTravel.Services.Data.Models.Countries;

    public interface ICountriesService
    {
        IEnumerable<CountryViewModel> GetAll();
    }
}

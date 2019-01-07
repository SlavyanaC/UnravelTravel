namespace UnravelTravel.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.Countries;

    public interface ICountriesService
    {
        Task<IEnumerable<CountryViewModel>> GetAllAsync();
    }
}

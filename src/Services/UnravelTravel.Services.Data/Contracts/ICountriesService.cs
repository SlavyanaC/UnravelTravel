namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Countries;

    public interface ICountriesService
    {
        Task<CountryViewModel[]> GetAllAsync();
    }
}

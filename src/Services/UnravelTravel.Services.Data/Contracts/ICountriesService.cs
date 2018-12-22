namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.Countries;

    public interface ICountriesService
    {
        Task<CountryViewModel[]> GetAllAsync();
    }
}

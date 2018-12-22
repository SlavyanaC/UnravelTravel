namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.Locations;

    public interface ILocationsService
    {
        Task<int> CreateAsync(params object[] parameters);

        Task<LocationViewModel[]> GetAllLocationsAsync();
    }
}

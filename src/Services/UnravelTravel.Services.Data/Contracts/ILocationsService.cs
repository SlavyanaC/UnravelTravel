namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Locations;

    public interface ILocationsService
    {
        Task<int> CreateAsync(params object[] parameters);

        Task<LocationViewModel[]> GetAllLocationsAsync();
    }
}

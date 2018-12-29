namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.AdministratorInputModels.Locations;
    using UnravelTravel.Models.ViewModels.Locations;

    public interface ILocationsService
    {
        Task<LocationViewModel> CreateLocationAsync(LocationCreateInputModel locationCreateInputModel);

        Task<LocationViewModel[]> GetAllLocationsAsync();
    }
}

namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Destinations;

    public interface IDestinationsService : ICrudDataService
    {
        Task<DestinationViewModel[]> GetAllDestinationsAsync();
    }
}

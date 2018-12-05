namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Destinations;

    public interface IDestinationsService
    {
        Task<int> CreateAsync(string name, int countryId, string imageUrl, string information);

        Task<DestinationDetailsViewModel> GetDestinationDetailsAsync(int id);

        Task<AllDestinationsViewModel> GetAllDestinationsAsync();

        Task<DestinationEditViewModel> GetDestinationToEditAsync(int id);

        Task EditDestinationAsync(int id, string name, int countryId, string imageUrl, string information);

        Task DeleteDestination(int id);
    }
}

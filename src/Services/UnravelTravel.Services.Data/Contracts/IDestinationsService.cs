namespace UnravelTravel.Services.Data.Contracts
{
    using System;
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Models.ViewModels.Home;

    public interface IDestinationsService : ICrudDataService
    {
        Task<DestinationViewModel[]> GetAllDestinationsAsync();

        Task<SearchResultViewModel> GetSearchResult(int destinationId, DateTime startDate, DateTime endDate);
    }
}

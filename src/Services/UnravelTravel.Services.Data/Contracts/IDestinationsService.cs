namespace UnravelTravel.Services.Data.Contracts
{
    using System;
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Destinations;
    using UnravelTravel.Services.Data.Models.Home;

    public interface IDestinationsService : ICrudDataService
    {
        Task<DestinationViewModel[]> GetAllDestinationsAsync();

        Task<SearchResultViewModel> GetSearchResult(int destinationId, DateTime startDate, DateTime endDate);
    }
}

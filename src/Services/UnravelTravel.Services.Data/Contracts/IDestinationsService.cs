namespace UnravelTravel.Services.Data.Contracts
{
    using System;
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Models.ViewModels.Home;

    public interface IDestinationsService : IBaseDataService
    {
        Task<DestinationDetailsViewModel> CreateAsync(DestinationCreateInputModel destinationCreateInputModel);

        Task EditAsync(DestinationEditViewModel destinationEditViewModel);

        Task<DestinationViewModel[]> GetAllDestinationsAsync();

        Task<SearchResultViewModel> GetSearchResultAsync(int destinationId, DateTime startDate, DateTime endDate);
    }
}

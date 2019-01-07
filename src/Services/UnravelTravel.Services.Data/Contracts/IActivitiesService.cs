namespace UnravelTravel.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.AdministratorInputModels.Activities;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Models.ViewModels.Enums;

    public interface IActivitiesService : IBaseDataService
    {
        Task<ActivityDetailsViewModel> CreateAsync(ActivityCreateInputModel activityCreateInputModel);

        Task<IEnumerable<ActivityViewModel>> GetAllAsync();

        Task<IEnumerable<ActivityViewModel>> GetAllInDestinationAsync(int destinationId);

        Task EditAsync(ActivityEditViewModel activityToEditViewModel);

        Task Review(int activityId, string username, ActivityReviewInputModel activityReviewInputModel);

        IEnumerable<ActivityViewModel> GetActivitiesFromSearch(string searchString, int? destinationId);

        IEnumerable<ActivityViewModel> SortBy(ActivityViewModel[] activities, ActivitiesSorter sorter);
    }
}

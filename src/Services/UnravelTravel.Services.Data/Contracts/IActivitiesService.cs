namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.Activities;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Activities;
    using UnravelTravel.Models.ViewModels.Activities;

    public interface IActivitiesService : IBaseDataService
    {
        Task<int> CreateAsync(ActivityCreateInputModel activityCreateInputModel);

        Task<ActivityViewModel[]> GetAllAsync();

        Task EditAsync(ActivityToEditViewModel activityToEditViewModel);

        Task Review(int activityId, string username, ActivityReviewInputModel activityReviewInputModel);
    }
}

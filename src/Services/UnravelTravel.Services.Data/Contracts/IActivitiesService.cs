namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.AdministratorInputModels.Activities;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;

    public interface IActivitiesService : IBaseDataService
    {
        Task<ActivityDetailsViewModel> CreateAsync(ActivityCreateInputModel activityCreateInputModel);

        Task<ActivityViewModel[]> GetAllAsync();

        Task EditAsync(ActivityEditViewModel activityToEditViewModel);

        Task Review(int activityId, string username, ActivityReviewInputModel activityReviewInputModel);
    }
}

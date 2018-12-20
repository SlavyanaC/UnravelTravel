namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Activities;

    public interface IActivitiesService : ICrudDataService
    {
        Task<ActivityViewModel[]> GetAllAsync();

        Task Review(int id, string username, params object[] parameters);
    }
}

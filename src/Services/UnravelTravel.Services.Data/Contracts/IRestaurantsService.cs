namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Restaurants;

    public interface IRestaurantsService : ICrudDataService
    {
        Task<RestaurantViewModel[]> GetAllRestaurantsAsync();

        Task Review(int id, string username, params object[] parameters);
    }
}

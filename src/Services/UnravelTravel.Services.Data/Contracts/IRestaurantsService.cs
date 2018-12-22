namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants;
    using UnravelTravel.Models.InputModels.Restaurants;
    using UnravelTravel.Models.ViewModels.Restaurants;

    public interface IRestaurantsService : IBaseDataService
    {
        Task<int> CreateAsync(RestaurantCreateInputModel restaurantCreateInputModel);

        Task<RestaurantViewModel[]> GetAllAsync();

        Task EditAsync(RestaurantEditViewModel restaurantEditViewModel);

        Task Review(int restaurantId, string username, RestaurantReviewInputModel restaurantReviewInputModel);
    }
}

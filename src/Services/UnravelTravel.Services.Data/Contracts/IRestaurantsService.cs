namespace UnravelTravel.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Enums;
    using UnravelTravel.Models.ViewModels.Restaurants;

    public interface IRestaurantsService : IBaseDataService
    {
        Task<RestaurantDetailsViewModel> CreateAsync(RestaurantCreateInputModel restaurantCreateInputModel);

        Task<RestaurantViewModel[]> GetAllAsync();

        Task EditAsync(RestaurantEditViewModel restaurantEditViewModel);

        Task Review(int restaurantId, string username, RestaurantReviewInputModel restaurantReviewInputModel);

        Task<RestaurantViewModel[]> GetAllInDestinationAsync(int destinationId);

        IEnumerable<RestaurantViewModel> GetRestaurantsFromSearch(string searchString, int? destinationId);

        IEnumerable<RestaurantViewModel> SortBy(RestaurantViewModel[] restaurants, RestaurantSorter sorter);
    }
}

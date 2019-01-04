namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;

    public interface IShoppingCartsService
    {
        Task AssignShoppingCartsUserId(UnravelTravelUser user);

        Task<ShoppingCartActivityViewModel[]> GetAllShoppingCartActivitiesAsync(string username);

        Task AddActivityToShoppingCartAsync(int activityId, string username, int quantity);

        Task<ShoppingCartActivityViewModel> GetGuestShoppingCartActivityToAdd(int shoppingCartActivityId, int quantity);

        Task EditShoppingCartActivityAsync(int shoppingCartActivityId, string username, int newQuantity);

        ShoppingCartActivityViewModel[] EditGuestShoppingCartActivity(int shoppingCartActivityId, ShoppingCartActivityViewModel[] cart, int newQuantity);

        Task DeleteActivityFromShoppingCart(int shoppingCartActivityId, string username);

        ShoppingCartActivityViewModel[] DeleteActivityFromGuestShoppingCart(int shoppingCartActivityId, ShoppingCartActivityViewModel[] cart);

        Task ClearShoppingCart(string username);
    }
}

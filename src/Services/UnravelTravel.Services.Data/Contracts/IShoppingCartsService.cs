namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.ShoppingCart;

    public interface IShoppingCartsService
    {
        Task<ShoppingCartActivityViewModel[]> GetAllTickets(string username);

        Task AddActivityToShoppingCart(int activityId, string username, int quantity);

        Task<ShoppingCartActivityViewModel> GetGuestShoppingCartActivityToAdd(int activityId, int quantity);

        Task EditShoppingCartActivity(int activityId, string username, int newQuantity);

        ShoppingCartActivityViewModel[] EditGuestShoppingCartActivity(int shoppingCartActivityId, ShoppingCartActivityViewModel[] cart, int newQuantity);

        Task DeleteActivityFromShoppingCart(int activityId, string username);

        ShoppingCartActivityViewModel[] DeleteActivityFromGuestShoppingCart(int shoppingCartActivityId, ShoppingCartActivityViewModel[] cart);

        Task ClearShoppingCart(string username);
    }
}

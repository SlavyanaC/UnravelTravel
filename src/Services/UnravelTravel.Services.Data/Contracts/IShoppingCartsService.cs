namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.ShoppingCart;

    public interface IShoppingCartsService
    {
        Task<ShoppingCartActivityViewModel[]> GetAllTickets(string username);

        Task AddActivityToShoppingCart(int activityId, string username, int quantity);

        Task DeleteActivityFromShoppingCart(int shoppingCartActivityId, string username);

        Task EditShoppingCartActivity(int shoppingCartActivityId, string username, int newQuantity);

        Task ClearShoppingCart(string username);
    }
}

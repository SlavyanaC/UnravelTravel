namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.ShoppingCart;
    using UnravelTravel.Services.Data.Models.Tickets;

    public interface ITicketsService
    {
        Task<TicketDetailsViewModel> GetDetailsAsync(int id);

        Task<TicketDetailsViewModel[]> GetAllAsync(string username);

        Task BookAllAsync(string username, ShoppingCartActivityViewModel[] shoppingCartActivities);
    }
}

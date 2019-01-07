namespace UnravelTravel.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Models.ViewModels.Tickets;

    public interface ITicketsService
    {
        Task<TicketDetailsViewModel> GetDetailsAsync(int id);

        Task<IEnumerable<TicketDetailsViewModel>> GetAllAsync(string username);

        Task BookAllAsync(string userIdentifier, ShoppingCartActivityViewModel[] shoppingCartActivities, string paymentMethod = "");
    }
}

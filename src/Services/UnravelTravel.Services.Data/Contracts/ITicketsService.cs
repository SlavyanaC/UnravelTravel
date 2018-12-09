namespace UnravelTravel.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using UnravelTravel.Services.Data.Models.Tickets;

    public interface ITicketsService
    {
        Task<int> BookAsync(string username, int activityId);

        Task<TicketDetailsViewModel> GetDetailsAsync(int id);

        Task<TicketDetailsViewModel[]> GetAllAsync(string username);
    }
}

namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.ShoppingCart;
    using UnravelTravel.Services.Data.Models.Tickets;
    using UnravelTravel.Services.Mapping;

    public class TicketsService : ITicketsService
    {
        private readonly IRepository<Ticket> ticketsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IRepository<ShoppingCartActivity> shoppingCartActivitiesRepository;
        private readonly IShoppingCartsService shoppingCartsService;

        public TicketsService(IRepository<Ticket> ticketsRepository, IRepository<ApplicationUser> usersRepository, IRepository<Activity> activitiesRepository, IRepository<ShoppingCartActivity> shoppingCartActivitiesRepository, IShoppingCartsService shoppingCartsService)
        {
            this.ticketsRepository = ticketsRepository;
            this.usersRepository = usersRepository;
            this.activitiesRepository = activitiesRepository;
            this.shoppingCartActivitiesRepository = shoppingCartActivitiesRepository;
            this.shoppingCartsService = shoppingCartsService;
        }

        public async Task BookAllAsync(string username, ShoppingCartActivityViewModel[] shoppingCartActivities)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException($"User with username {username} not found.");
            }

            foreach (var shoppingCartActivity in shoppingCartActivities)
            {
                var shoppingCartActivityDb = await this.shoppingCartActivitiesRepository.All()
                    .FirstOrDefaultAsync(sca => sca.Id == shoppingCartActivity.Id);
                if (shoppingCartActivityDb == null)
                {
                    throw new NullReferenceException($"Shopping cart activity with id {shoppingCartActivity.Id} not found.");
                }

                var activityId = shoppingCartActivity.ActivityId;
                var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == activityId);
                if (activity == null)
                {
                    throw new NullReferenceException($"Activity with id {activityId} not found.");
                }

                await this.BookAsync(user, activity);
                await this.shoppingCartsService.ClearShoppingCart(username);
            }
        }

        public async Task<TicketDetailsViewModel> GetDetailsAsync(int id)
        {
            var ticketDetailsViewModel = await this.ticketsRepository
                .All()
                .To<TicketDetailsViewModel>()
                .FirstOrDefaultAsync(t => t.Id == id);

            return ticketDetailsViewModel;
        }

        public async Task<TicketDetailsViewModel[]> GetAllAsync(string username)
        {
            var ticketsViewModel = await this.ticketsRepository
                .All()
                .Where(t => t.User.UserName == username)
                .To<TicketDetailsViewModel>()
                .ToArrayAsync();

            return ticketsViewModel;
        }

        private async Task BookAsync(ApplicationUser user, Activity activity)
        {
            var ticket = new Ticket
            {
                User = user,
                Activity = activity,
            };

            this.ticketsRepository.Add(ticket);
            await this.ticketsRepository.SaveChangesAsync();
        }
    }
}

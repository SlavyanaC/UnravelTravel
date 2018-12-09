using System.Linq;
using UnravelTravel.Common.Mapping;
using UnravelTravel.Services.Data.Models.Tickets;

namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;

    public class TicketsService : ITicketsService
    {
        private readonly IRepository<Ticket> ticketsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;

        public TicketsService(IRepository<Ticket> ticketsRepository, IRepository<ApplicationUser> usersRepository)
        {
            this.ticketsRepository = ticketsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task<int> BookAsync(string username, int activityId)
        {
            var user = await this.usersRepository
                .All()
                .FirstOrDefaultAsync(u => u.UserName == username);
            var ticket = new Ticket
            {
                User = user,
                ActivityId = activityId,
            };

            this.ticketsRepository.Add(ticket);
            await this.ticketsRepository.SaveChangesAsync();

            return ticket.Id;
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
    }
}

namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Models.ViewModels.Tickets;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public class TicketsService : ITicketsService
    {
        private readonly IRepository<Ticket> ticketsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IShoppingCartsService shoppingCartsService;
        private readonly IEmailSender emailSender;

        public TicketsService(
            IRepository<Ticket> ticketsRepository,
            IRepository<ApplicationUser> usersRepository,
            IRepository<Activity> activitiesRepository,
            IShoppingCartsService shoppingCartsService,
            IEmailSender emailSender)
        {
            this.ticketsRepository = ticketsRepository;
            this.usersRepository = usersRepository;
            this.activitiesRepository = activitiesRepository;
            this.shoppingCartsService = shoppingCartsService;
            this.emailSender = emailSender;
        }

        public async Task<TicketDetailsViewModel> GetDetailsAsync(int id)
        {
            var ticketDetailsViewModel = await this.ticketsRepository
                .All()
                .To<TicketDetailsViewModel>()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticketDetailsViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceTicketId, id));
            }

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

        public async Task BookAllAsync(string username, ShoppingCartActivityViewModel[] shoppingCartActivities, string paymentMethod = "")
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var ticketIds = new List<int>();
            foreach (var shoppingCartActivity in shoppingCartActivities)
            {
                var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == shoppingCartActivity.ActivityId);
                if (activity == null)
                {
                    throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, shoppingCartActivity.ActivityId));
                }

                if (shoppingCartActivity.Quantity <= 0)
                {
                    throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
                }

                var ticketId = await this.GetBookingId(user, activity, shoppingCartActivity.Quantity);
                ticketIds.Add(ticketId);
            }

            await this.shoppingCartsService.ClearShoppingCart(username);

            var content = await this.GenerateEmailContent(ticketIds, paymentMethod);
            await this.emailSender.SendEmailAsync(
                user.Email,
                "Booking confirmation",
                content);
        }

        private async Task<int> GetBookingId(ApplicationUser user, Activity activity, int quantity)
        {
            var ticket = new Ticket
            {
                User = user,
                Activity = activity,
                Quantity = quantity,
            };

            this.ticketsRepository.Add(ticket);
            await this.ticketsRepository.SaveChangesAsync();

            return ticket.Id;
        }

        private async Task<string> GenerateEmailContent(List<int> ticketIds, string paymentMethod)
        {
            var ticketsInfo = new StringBuilder();
            var totalPrice = 0m;
            foreach (var ticketId in ticketIds)
            {
                var ticket = await this.ticketsRepository.All().FirstOrDefaultAsync(t => t.Id == ticketId);
                if (ticket == null)
                {
                    throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceTicketId, ticketId));
                }

                var activityName = ticket.Activity.Name;
                var ticketQuantity = ticket.Quantity;
                var activityPrice = ticket.Activity.Price.Value;

                var str = $@"<tr>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{ticketId}</td>
                     <td align=""left"" width=""50%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{activityName}</td>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">{ticketQuantity}</td>
                     <td align=""left"" width=""25%"" style=""padding: 6px 12px;font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px;"">${activityPrice:F2}</td>
                     </tr>";

                ticketsInfo.Append(str);
                totalPrice += activityPrice;
            }

            var total = $@"<tr>
				  <td align=""left"" width=""50%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>Total</strong></td>
				  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
				  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong></strong></td>
                  <td align=""left"" width=""25%"" style=""padding: 12px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; line-height: 24px; border-top: 2px dashed #D2C7BA; border-bottom: 2px dashed #D2C7BA;""><strong>${totalPrice:F2}</strong></td>
                </tr>";

            var receiptPath =
                @"C:\Users\Slavi\Documents\Project\UnravelTravel\src\Services\UnravelTravel.Services.Data\Common\receipt.html";

            var doc = new HtmlDocument();
            doc.Load(receiptPath);

            paymentMethod = paymentMethod == "online" ? "Payed online" : "Pay when you get there";

            var content = doc.Text;
            content = content.Replace("@ticketsInfo", ticketsInfo.ToString())
                .Replace("@totalInfo", total)
                .Replace("@paymentMethod", paymentMethod);

            return content;
        }
    }
}

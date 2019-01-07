namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common;
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
        private readonly IRepository<UnravelTravelUser> usersRepository;
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IShoppingCartsService shoppingCartsService;
        private readonly IEmailSender emailSender;

        public TicketsService(
            IRepository<Ticket> ticketsRepository,
            IRepository<UnravelTravelUser> usersRepository,
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

        public async Task BookAllAsync(string userIdentifier, ShoppingCartActivityViewModel[] shoppingCartActivities, string paymentMethod = "")
        {
            // If identifier is email the user is guest
            var isGuest = Regex.IsMatch(userIdentifier, @"[^@]+@[^\.]+\..+"); 
            if (isGuest)
            {
                await this.BookGuestUsersTickets(userIdentifier, shoppingCartActivities, paymentMethod);
            }
            else
            {
                await this.BookUsersTickets(userIdentifier, shoppingCartActivities, paymentMethod);
            }
        }

        private async Task BookUsersTickets(string userIdentifier, IEnumerable<ShoppingCartActivityViewModel> shoppingCartActivities, string paymentMethod)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == userIdentifier);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, userIdentifier));
            }

            var ticketIds = new List<int>();
            foreach (var shoppingCartActivity in shoppingCartActivities)
            {
                var activity = await this.activitiesRepository.All()
                    .FirstOrDefaultAsync(a => a.Id == shoppingCartActivity.ActivityId);
                if (activity == null)
                {
                    throw new NullReferenceException(string.Format(
                        ServicesDataConstants.NullReferenceActivityId,
                        shoppingCartActivity.ActivityId));
                }

                if (shoppingCartActivity.Quantity <= 0)
                {
                    throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
                }

                var ticketId = await this.GetBookingId(user.Id, activity, shoppingCartActivity.Quantity);
                ticketIds.Add(ticketId);
            }

            await this.shoppingCartsService.ClearShoppingCart(userIdentifier);

            var emailContent = await this.GenerateEmailContent(ticketIds, paymentMethod);
            await this.emailSender.SendEmailAsync(
                user.Email,
                ServicesDataConstants.BookingEmailSubject,
                emailContent);
        }

        private async Task BookGuestUsersTickets(string userIdentifier, IEnumerable<ShoppingCartActivityViewModel> shoppingCartActivities, string paymentMethod)
        {
            var ticketIds = new List<int>();
            foreach (var shoppingCartActivity in shoppingCartActivities)
            {
                var activity = await this.activitiesRepository.All()
                    .FirstOrDefaultAsync(a => a.Id == shoppingCartActivity.ActivityId);
                if (activity == null)
                {
                    throw new NullReferenceException(string.Format(
                        ServicesDataConstants.NullReferenceActivityId,
                        shoppingCartActivity.ActivityId));
                }

                if (shoppingCartActivity.Quantity <= 0)
                {
                    throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
                }

                var ticketId = await this.GetBookingId(null, activity, shoppingCartActivity.Quantity);
                ticketIds.Add(ticketId);
            }

            var emailContent = await this.GenerateEmailContent(ticketIds, paymentMethod);
            await this.emailSender.SendEmailAsync(
                userIdentifier,
                ServicesDataConstants.BookingEmailSubject,
                emailContent);
        }

        private async Task<int> GetBookingId(string userId, Activity activity, int quantity)
        {
            var ticket = new Ticket
            {
                UserId = userId,
                Activity = activity,
                Quantity = quantity,
            };

            this.ticketsRepository.Add(ticket);
            await this.ticketsRepository.SaveChangesAsync();

            return ticket.Id;
        }

        private async Task<string> GenerateEmailContent(IEnumerable<int> ticketIds, string paymentMethod)
        {
            var ticketsInfo = new StringBuilder();
            var totalPrice = 0m;
            foreach (var ticketId in ticketIds)
            {
                var ticket = await this.ticketsRepository.All().FirstOrDefaultAsync(t => t.Id == ticketId);

                var activityName = ticket.Activity.Name;
                var ticketQuantity = ticket.Quantity;
                var activityPrice = ticket.Activity.Price * ticket.Quantity;

                var ticketInfoHtml = string.Format(
                    ServicesDataConstants.TicketActivityReceiptHtmlInfo,
                    ticketId,
                    activityName,
                    ticketQuantity,
                    activityPrice);

                ticketsInfo.Append(ticketInfoHtml);
                totalPrice += activityPrice;
            }

            var totalInfoHtml = string.Format(ServicesDataConstants.TotalReceiptHtmlInfo, totalPrice);

            var receiptPath = ServicesDataConstants.TicketsReceiptEmailHtmlPath;
            var doc = new HtmlDocument();
            doc.Load(receiptPath);

            paymentMethod = paymentMethod == GlobalConstants.OnlinePaymentMethod ?
                ServicesDataConstants.OnlinePaymentEmailString :
                ServicesDataConstants.CashPaymentEmailString;

            var content = doc.Text;
            content = content.Replace(ServicesDataConstants.TicketsInfoPlaceholder, ticketsInfo.ToString())
                .Replace(ServicesDataConstants.TotalReceiptInfoPlaceholder, totalInfoHtml)
                .Replace(ServicesDataConstants.PaymentMethodPlaceholder, paymentMethod);

            return content;
        }
    }
}

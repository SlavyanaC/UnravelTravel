namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.InputModels.Reservations;
    using UnravelTravel.Models.ViewModels.Reservations;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    using AutoMap = AutoMapper;

    public class ReservationsService : IReservationsService
    {
        private readonly IRepository<Reservation> reservationsRepository;
        private readonly IRepository<UnravelTravelUser> usersRepository;
        private readonly IRepository<Restaurant> restaurantsRepository;
        private readonly IEmailSender emailSender;

        public ReservationsService(IRepository<Reservation> reservationsRepository, IRepository<UnravelTravelUser> usersRepository, IRepository<Restaurant> restaurantsRepository, IEmailSender emailSender)
        {
            this.usersRepository = usersRepository;
            this.restaurantsRepository = restaurantsRepository;
            this.reservationsRepository = reservationsRepository;
            this.emailSender = emailSender;
        }

        public async Task<ReservationDetailsViewModel> BookAsync(int restaurantId, string userIdentifier, ReservationCreateInputModel reservationCreateInputModel)
        {
            // If identifier is email the user is guest
            var isGuest = Regex.IsMatch(userIdentifier, ServicesDataConstants.EmailRegex);
            UnravelTravelUser user = null;
            if (!isGuest)
            {
                user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == userIdentifier);
                if (user == null)
                {
                    throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, userIdentifier));
                }
            }

            var restaurant = await this.restaurantsRepository.All().FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceRestaurantId, restaurantId));
            }

            Reservation reservation = null;
            if (!isGuest)
            {
                reservation = await this.reservationsRepository.All()
                    .FirstOrDefaultAsync(r => r.User == user &&
                                              r.Restaurant == restaurant &&
                                              r.Date == reservationCreateInputModel.Date);

                if (reservation != null)
                {
                    reservation.PeopleCount += reservationCreateInputModel.PeopleCount;
                    this.reservationsRepository.Update(reservation);
                }
            }

            if (reservation == null)
            {
                // var utcReservationDate = reservationCreateInputModel.Date.GetUtcDate(
                //    restaurant.Destination.Name,
                //    restaurant.Destination.Country.Name);
                var utcReservationDate =
                    reservationCreateInputModel.Date.CalculateUtcDateTime(restaurant.Destination.UtcRawOffset);

                reservation = new Reservation
                {
                    UserId = user == null ? null : user.Id,
                    Restaurant = restaurant,
                    Date = utcReservationDate, // Save UTC date to Db
                    PeopleCount = reservationCreateInputModel.PeopleCount,
                };

                this.reservationsRepository.Add(reservation);
                await this.reservationsRepository.SaveChangesAsync();
            }

            var reservationDetailsViewModel = AutoMap.Mapper.Map<ReservationDetailsViewModel>(reservation);

            var emailContent = await this.GenerateEmailContent(reservationDetailsViewModel);
            await this.emailSender.SendEmailAsync(
                user != null ? user.Email : userIdentifier,
                ServicesDataConstants.BookingEmailSubject,
                emailContent);

            return reservationDetailsViewModel;
        }

        public async Task<ReservationDetailsViewModel> GetDetailsAsync(int id)
        {
            var reservationDetailsViewModel = await this.reservationsRepository
                .All()
                .To<ReservationDetailsViewModel>()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservationDetailsViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceReservationId, id));
            }

            return reservationDetailsViewModel;
        }

        public async Task<IEnumerable<ReservationDetailsViewModel>> GetAllAsync(string username)
        {
            var allReservations = await this.reservationsRepository
                .All()
                .Where(r => r.User.UserName == username)
                .To<ReservationDetailsViewModel>()
                .ToArrayAsync();

            return allReservations;
        }

        private async Task<string> GenerateEmailContent(ReservationDetailsViewModel reservationDetailsViewModel)
        {
            var reservationInfoHtml = string.Format(
                ServicesDataConstants.ReservationHtmlInfo,
                reservationDetailsViewModel.Id,
                reservationDetailsViewModel.RestaurantName,
                reservationDetailsViewModel.PeopleCount,
                reservationDetailsViewModel.ReservationDayString + " " + reservationDetailsViewModel.ReservationHourString);

            var restaurant = await this.restaurantsRepository.All()
                .FirstOrDefaultAsync(r => r.Id == reservationDetailsViewModel.RestaurantId);

            var restaurantInfoHtml = string.Format(
                ServicesDataConstants.RestaurantHtmlInfo,
                restaurant.Address,
                restaurant.Destination.Name,
                restaurant.Destination.Country.Name);

            var receiptPath = ServicesDataConstants.ReservationReceiptEmailHtmlPath;
            var doc = new HtmlDocument();
            doc.Load(receiptPath);

            var content = doc.Text;
            content = content.Replace(ServicesDataConstants.ReservationInfoPlaceholder, reservationInfoHtml)
                .Replace(ServicesDataConstants.RestaurantInfoPlaceholder, restaurantInfoHtml);

            return content;
        }
    }
}

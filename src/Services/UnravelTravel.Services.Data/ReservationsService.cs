using System.Collections.Generic;

namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
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

        public async Task<ReservationDetailsViewModel> BookAsync(int restaurantId, string username, ReservationCreateInputModel reservationCreateInputModel)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var restaurant = await this.restaurantsRepository.All().FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceRestaurantId, restaurantId));
            }

            var reservation = await this.reservationsRepository.All().FirstOrDefaultAsync(r =>
                r.User == user && r.Restaurant == restaurant && r.Date == reservationCreateInputModel.Date);
            if (reservation != null)
            {
                reservation.PeopleCount += reservationCreateInputModel.PeopleCount;
                this.reservationsRepository.Update(reservation);
            }
            else
            {
                reservation = new Reservation
                {
                    User = user,
                    Restaurant = restaurant,
                    Date = reservationCreateInputModel.Date,
                    PeopleCount = reservationCreateInputModel.PeopleCount,
                };

                this.reservationsRepository.Add(reservation);
            }

            await this.reservationsRepository.SaveChangesAsync();

            var reservationDetailsViewModel = AutoMap.Mapper.Map<ReservationDetailsViewModel>(reservation);

            var emailContent = await this.GenerateEmailContent(reservationDetailsViewModel);
            await this.emailSender.SendEmailAsync(
                user.Email,
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

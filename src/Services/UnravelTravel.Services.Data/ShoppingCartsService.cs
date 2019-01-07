namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public class ShoppingCartsService : IShoppingCartsService
    {
        private readonly IRepository<UnravelTravelUser> usersRepository;
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IRepository<ShoppingCartActivity> shoppingCartActivitiesRepository;
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;

        public ShoppingCartsService(
            IRepository<UnravelTravelUser> usersRepository,
            IRepository<Activity> activitiesRepository,
            IRepository<ShoppingCartActivity> shoppingCartActivitiesRepository,
            IRepository<ShoppingCart> shoppingCartsRepository)
        {
            this.usersRepository = usersRepository;
            this.activitiesRepository = activitiesRepository;
            this.shoppingCartActivitiesRepository = shoppingCartActivitiesRepository;
            this.shoppingCartsRepository = shoppingCartsRepository;
        }

        // TODO: this is stupid but db does not store userId for each shopping cart find out why!
        public async Task AssignShoppingCartsUserId(UnravelTravelUser user)
        {
            var shoppingCart = await this.shoppingCartsRepository.All()
                .FirstOrDefaultAsync(sc => sc.User.Id == user.Id);
            if (shoppingCart == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartForUser, user.Id, user.UserName));
            }

            shoppingCart.UserId = user.Id;
            this.shoppingCartsRepository.Update(shoppingCart);
            await this.shoppingCartActivitiesRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingCartActivityViewModel>> GetAllShoppingCartActivitiesAsync(string username)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var shoppingCartTickets = await this.shoppingCartActivitiesRepository
                .All()
                .Where(sca => sca.ShoppingCart.User.UserName == username)
                .To<ShoppingCartActivityViewModel>()
                .ToArrayAsync();

            return shoppingCartTickets;
        }

        public async Task<ShoppingCartActivityViewModel> GetGuestShoppingCartActivityToAdd(int activityId, int quantity)
        {
            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == activityId);
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, activityId));
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            // TODO: AutoMapper?
            var shoppingCartActivity = new ShoppingCartActivityViewModel
            {
                ActivityId = activity.Id,
                ActivityName = activity.Name,
                ActivityDate = activity.Date,
                ActivityDestinationName = activity.Destination.Name,
                ActivityImageUrl = activity.ImageUrl,
                ActivityPrice = activity.Price,
                Quantity = quantity,
            };

            return shoppingCartActivity;
        }

        public async Task AddActivityToShoppingCartAsync(int activityId, string username, int quantity)
        {
            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == activityId);
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, activityId));
            }

            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            var shoppingCartActivity = new ShoppingCartActivity
            {
                Activity = activity,
                ShoppingCart = user.ShoppingCart,
                Quantity = quantity,
            };

            this.shoppingCartActivitiesRepository.Add(shoppingCartActivity);
            await this.shoppingCartActivitiesRepository.SaveChangesAsync();
        }

        public async Task DeleteActivityFromShoppingCart(int shoppingCartActivityId, string username)
        {
            var shoppingCartActivity = await this.shoppingCartActivitiesRepository.All()
                .FirstOrDefaultAsync(sca => sca.Id == shoppingCartActivityId);
            if (shoppingCartActivity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartActivityId, shoppingCartActivityId));
            }

            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            shoppingCartActivity.IsDeleted = true;

            this.shoppingCartActivitiesRepository.Update(shoppingCartActivity);
            await this.shoppingCartActivitiesRepository.SaveChangesAsync();
        }

        public IEnumerable<ShoppingCartActivityViewModel> DeleteActivityFromGuestShoppingCart(int activityId, ShoppingCartActivityViewModel[] cart)
        {
            var shoppingCartActivityViewModel = cart.FirstOrDefault(sca => sca.ActivityId == activityId);

            if (shoppingCartActivityViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceGuestShoppingCartActivityId, activityId));
            }

            var activity = this.activitiesRepository.All().FirstOrDefault(a => a.Id == activityId);
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, activityId));
            }

            // TODO: use List<ShoppingCartActivityViewModel>
            var cartAsList = cart.ToList();
            cartAsList.Remove(shoppingCartActivityViewModel);
            cart = cartAsList.ToArray();
            return cart;
        }

        public async Task EditShoppingCartActivityAsync(int shoppingCartActivityId, string username, int newQuantity)
        {
            var shoppingCartActivity = await this.shoppingCartActivitiesRepository.All()
                .FirstOrDefaultAsync(sca => sca.Id == shoppingCartActivityId);
            if (shoppingCartActivity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartActivityId, shoppingCartActivityId));
            }

            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            if (newQuantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            shoppingCartActivity.Quantity = newQuantity;
            this.shoppingCartActivitiesRepository.Update(shoppingCartActivity);
            await this.shoppingCartActivitiesRepository.SaveChangesAsync();
        }

        public IEnumerable<ShoppingCartActivityViewModel> EditGuestShoppingCartActivity(int shoppingCartActivityId, ShoppingCartActivityViewModel[] cart, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            var shoppingCartActivityViewModel = cart.FirstOrDefault(sca => sca.ActivityId == shoppingCartActivityId);
            if (shoppingCartActivityViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceGuestShoppingCartActivityId, shoppingCartActivityId));
            }

            var activity = this.activitiesRepository.All().FirstOrDefault(a => a.Id == shoppingCartActivityId);
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, shoppingCartActivityId));
            }

            shoppingCartActivityViewModel.Quantity = newQuantity;

            for (var i = 0; i < cart.Length; i++)
            {
                if (cart[i].Id == shoppingCartActivityId)
                {
                    cart[i] = shoppingCartActivityViewModel;
                }
            }

            return cart;
        }

        public async Task ClearShoppingCart(string username)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var shoppingCartActivities = this.shoppingCartActivitiesRepository
                .All()
                .Where(sca => sca.ShoppingCart.User == user)
                .ToList();

            shoppingCartActivities.ForEach(sca => sca.IsDeleted = true);

            this.shoppingCartActivitiesRepository.UpdateRange(shoppingCartActivities);
            await this.shoppingCartActivitiesRepository.SaveChangesAsync();
        }
    }
}

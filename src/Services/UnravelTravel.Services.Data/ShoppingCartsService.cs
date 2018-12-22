namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public class ShoppingCartsService : IShoppingCartsService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IRepository<ShoppingCartActivity> shoppingCartActivitiesRepository;

        public ShoppingCartsService(IRepository<ApplicationUser> usersRepository, IRepository<Activity> activitiesRepository, IRepository<ShoppingCartActivity> shoppingCartActivitiesRepository)
        {
            this.usersRepository = usersRepository;
            this.activitiesRepository = activitiesRepository;
            this.shoppingCartActivitiesRepository = shoppingCartActivitiesRepository;
        }

        public async Task<ShoppingCartActivityViewModel[]> GetAllTickets(string username)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException($"User with username {username} not found!");
            }

            var shoppingCartTickets = await this.shoppingCartActivitiesRepository
                .All()
                .Where(sca => sca.ShoppingCart.User.UserName == username)
                .To<ShoppingCartActivityViewModel>()
                .ToArrayAsync();

            return shoppingCartTickets;
        }

        public async Task AddActivityToShoppingCart(int activityId, string username, int quantity)
        {
            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == activityId);
            if (activity == null)
            {
                throw new NullReferenceException($"Activity with id {activityId} not found.");
            }

            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException($"User with username {username} not found.");
            }

            if (quantity <= 0)
            {
                throw new ArgumentException($"Quantity cannot be negative or zero");
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
                throw new NullReferenceException($"Shopping cart activity with id {shoppingCartActivityId} not found.");
            }

            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException($"User with username {username} not found.");
            }

            shoppingCartActivity.IsDeleted = true;

            this.shoppingCartActivitiesRepository.Update(shoppingCartActivity);
            await this.shoppingCartActivitiesRepository.SaveChangesAsync();
        }

        public async Task EditShoppingCartActivity(int shoppingCartActivityId, string username, int newQuantity)
        {
            var shoppingCartActivity = await this.shoppingCartActivitiesRepository.All()
                .FirstOrDefaultAsync(sca => sca.Id == shoppingCartActivityId);
            if (shoppingCartActivity == null)
            {
                throw new NullReferenceException($"Shopping cart activity with id {shoppingCartActivityId} not found.");
            }

            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException($"User with username {username} not found.");
            }

            if (newQuantity <= 0)
            {
                throw new ArgumentException("Quantity cannot be negative or zero");
            }

            shoppingCartActivity.Quantity = newQuantity;
            this.shoppingCartActivitiesRepository.Update(shoppingCartActivity);
            await this.shoppingCartActivitiesRepository.SaveChangesAsync();
        }

        public async Task ClearShoppingCart(string username)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException($"User with username {username} not found.");
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

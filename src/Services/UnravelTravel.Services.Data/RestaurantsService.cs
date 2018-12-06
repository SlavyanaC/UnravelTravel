namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Destinations;
    using UnravelTravel.Services.Data.Models.Restaurants;

    public class RestaurantsService : IRestaurantsService
    {
        private readonly IRepository<Restaurant> restaurantsRepository;

        public RestaurantsService(IRepository<Restaurant> restaurantsRepository)
        {
            this.restaurantsRepository = restaurantsRepository;
        }

        public async Task<AllRestaurantsViewModel> GetAllRestaurantsAsync()
        {
            var restaurants = await this.restaurantsRepository
                .All()
                .To<RestaurantViewModel>()
                .ToListAsync();

            var allRestaurants = new AllRestaurantsViewModel()
            {
                Restaurants = restaurants,
            };

            return allRestaurants;
        }
    }
}

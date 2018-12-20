namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Restaurants;
    using UnravelTravel.Services.Data.Utilities;
    using UnravelTravel.Services.Mapping;

    public class RestaurantsService : IRestaurantsService
    {
        private readonly IRepository<Restaurant> restaurantsRepository;
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Review> reviewsRepository;
        private readonly IRepository<RestaurantReview> restaurantReviewsRepository;
        private readonly Cloudinary cloudinary;

        public RestaurantsService(IRepository<Restaurant> restaurantsRepository, IRepository<Destination> destinationsRepository, IRepository<ApplicationUser> usersRepository, IRepository<Review> reviewsRepository, IRepository<RestaurantReview> restaurantReviewsRepository, Cloudinary cloudinary)
        {
            this.restaurantsRepository = restaurantsRepository;
            this.destinationsRepository = destinationsRepository;
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.restaurantReviewsRepository = restaurantReviewsRepository;
            this.cloudinary = cloudinary;
        }

        public async Task<RestaurantViewModel[]> GetAllAsync()
        {
            var restaurants = await this.restaurantsRepository
                .All()
                .To<RestaurantViewModel>()
                .ToArrayAsync();

            return restaurants;
        }

        public async Task<int> CreateAsync(params object[] parameters)
        {
            var name = parameters[0].ToString();
            var address = parameters[1].ToString();
            var destinationId = int.Parse(parameters[2].ToString());
            var image = parameters[3] as IFormFile;
            var typeString = parameters[4].ToString();
            var seats = int.Parse(parameters[5].ToString());

            Enum.TryParse(typeString, true, out RestaurantType typeEnum);

            var imageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, image, name);

            var restaurant = new Restaurant()
            {
                Name = name,
                Address = address,
                DestinationId = destinationId,
                ImageUrl = imageUrl,
                Type = typeEnum,
                Seats = seats,
            };

            this.restaurantsRepository.Add(restaurant);
            await this.restaurantsRepository.SaveChangesAsync();

            return restaurant.Id;
        }

        public async Task<TViewModel> GetViewModelAsync<TViewModel>(int id)
        {
            var restaurant = await this.restaurantsRepository
                .All()
                .Where(d => d.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            return restaurant;
        }

        public async Task EditAsync(int id, params object[] parameters)
        {
            var name = parameters[0].ToString();
            var address = parameters[1].ToString();
            var destinationId = int.Parse(parameters[2].ToString());
            var newImage = parameters[3] as IFormFile;
            var seats = int.Parse(parameters[4].ToString());
            var type = parameters[5].ToString();

            Enum.TryParse(type, true, out RestaurantType typeEnum);

            var restaurant = this.restaurantsRepository.All().FirstOrDefault(r => r.Id == id);
            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == destinationId);

            if (newImage != null)
            {
                var newImageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, newImage, name);
                destination.ImageUrl = newImageUrl;
            }

            restaurant.Name = name;
            restaurant.Address = address;
            restaurant.DestinationId = destinationId;
            restaurant.Seats = seats;
            restaurant.Type = typeEnum;
            restaurant.Destination = destination;

            this.restaurantsRepository.Update(restaurant);
            await this.restaurantsRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var restaurant = this.restaurantsRepository.All().FirstOrDefault(d => d.Id == id);
            restaurant.IsDeleted = true;

            this.restaurantsRepository.Update(restaurant);
            await this.restaurantsRepository.SaveChangesAsync();
        }

        public async Task Review(int restaurantId, string username, params object[] parameters)
        {
            var rating = double.Parse(parameters[0].ToString());
            var content = parameters[1].ToString();

            var user = await this.usersRepository.All().Where(u => u.UserName == username).FirstOrDefaultAsync();

            var review = new Review
            {
                User = user,
                Rating = rating,
                Content = content,
            };

            this.reviewsRepository.Add(review);
            await this.restaurantsRepository.SaveChangesAsync();

            var restaurant = await this.restaurantsRepository.All().Where(r => r.Id == restaurantId).FirstOrDefaultAsync();

            var restaurantReview = new RestaurantReview
            {
                Restaurant = restaurant,
                Review = review,
            };

            this.restaurantReviewsRepository.Add(restaurantReview);
            await this.restaurantReviewsRepository.SaveChangesAsync();

            await this.UpdateRestaurantAverageRating(restaurant);
        }

        private async Task UpdateRestaurantAverageRating(Restaurant restaurant)
        {
            var avgRating = restaurant.Reviews.Average(ar => ar.Review.Rating);
            restaurant.AverageRating = avgRating;

            this.restaurantsRepository.Update(restaurant);
            await this.restaurantsRepository.SaveChangesAsync();
        }
    }
}

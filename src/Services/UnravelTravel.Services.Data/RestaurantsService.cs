namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants;
    using UnravelTravel.Models.InputModels.Restaurants;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Contracts;
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

        public async Task<int> CreateAsync(RestaurantCreateInputModel restaurantCreateInputModel)
        {
            Enum.TryParse(restaurantCreateInputModel.Type, true, out RestaurantType typeEnum);

            var imageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, restaurantCreateInputModel.Image, restaurantCreateInputModel.Name);

            var restaurant = new Restaurant()
            {
                Name = restaurantCreateInputModel.Name,
                Address = restaurantCreateInputModel.Address,
                DestinationId = restaurantCreateInputModel.DestinationId,
                ImageUrl = imageUrl,
                Type = typeEnum,
                Seats = restaurantCreateInputModel.Seats,
            };

            this.restaurantsRepository.Add(restaurant);
            await this.restaurantsRepository.SaveChangesAsync();

            return restaurant.Id;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var restaurant = await this.restaurantsRepository
                .All()
                .Where(d => d.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            return restaurant;
        }

        public async Task EditAsync(RestaurantEditViewModel restaurantEditViewModel)
        {
            Enum.TryParse(restaurantEditViewModel.Type, true, out RestaurantType restaurantTypeEnum);

            var restaurant = this.restaurantsRepository.All().FirstOrDefault(r => r.Id == restaurantEditViewModel.Id);
            if (restaurant == null)
            {
                throw new NullReferenceException($"Restaurant with id {restaurantEditViewModel.Id} not found.");
            }

            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == restaurantEditViewModel.DestinationId);
            if (destination == null)
            {
                throw new NullReferenceException($"Destination with id {restaurantEditViewModel.DestinationId} not found.");
            }

            if (restaurantEditViewModel.NewImage != null)
            {
                var newImageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, restaurantEditViewModel.NewImage, restaurantEditViewModel.Name);
                destination.ImageUrl = newImageUrl;
            }

            restaurant.Name = restaurantEditViewModel.Name;
            restaurant.Address = restaurantEditViewModel.Address;
            restaurant.Destination = destination;
            restaurant.Seats = restaurantEditViewModel.Seats;
            restaurant.Type = restaurantTypeEnum;

            this.restaurantsRepository.Update(restaurant);
            await this.restaurantsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var restaurant = this.restaurantsRepository.All().FirstOrDefault(d => d.Id == id);
            if (restaurant == null)
            {
                throw new NullReferenceException($"Restaurant with id {id} not found");
            }

            restaurant.IsDeleted = true;

            this.restaurantsRepository.Update(restaurant);
            await this.restaurantsRepository.SaveChangesAsync();
        }

        public async Task Review(int restaurantId, string username, RestaurantReviewInputModel restaurantReviewInputModel)
        {
            var user = await this.usersRepository.All().Where(u => u.UserName == username).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new NullReferenceException($"User with username {username} not found.");
            }

            var restaurant = await this.restaurantsRepository.All().Where(r => r.Id == restaurantId).FirstOrDefaultAsync();
            if (restaurant == null)
            {
                throw new NullReferenceException($"Restaurant with id {restaurantId} not found.");
            }

            var review = new Review
            {
                User = user,
                Rating = restaurantReviewInputModel.Rating,
                Content = restaurantReviewInputModel.Content,
            };

            this.reviewsRepository.Add(review);
            await this.restaurantsRepository.SaveChangesAsync();

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

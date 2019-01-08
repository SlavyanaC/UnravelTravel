namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Restaurants;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Enums;
    using UnravelTravel.Models.ViewModels.Restaurants;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    using AutoMap = AutoMapper;

    public class RestaurantsService : IRestaurantsService
    {
        private readonly IRepository<Restaurant> restaurantsRepository;
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<UnravelTravelUser> usersRepository;
        private readonly IRepository<Review> reviewsRepository;
        private readonly IRepository<RestaurantReview> restaurantReviewsRepository;
        private readonly Cloudinary cloudinary;

        public RestaurantsService(
            IRepository<Restaurant> restaurantsRepository,
            IRepository<Destination> destinationsRepository,
            IRepository<UnravelTravelUser> usersRepository,
            IRepository<Review> reviewsRepository,
            IRepository<RestaurantReview> restaurantReviewsRepository,
            Cloudinary cloudinary)
        {
            this.restaurantsRepository = restaurantsRepository;
            this.destinationsRepository = destinationsRepository;
            this.usersRepository = usersRepository;
            this.reviewsRepository = reviewsRepository;
            this.restaurantReviewsRepository = restaurantReviewsRepository;
            this.cloudinary = cloudinary;
        }

        public async Task<IEnumerable<RestaurantViewModel>> GetAllAsync()
        {
            var restaurants = await this.restaurantsRepository
                .All()
                .To<RestaurantViewModel>()
                .ToArrayAsync();
            return restaurants;
        }

        public async Task<IEnumerable<RestaurantViewModel>> GetAllInDestinationAsync(int destinationId)
        {
            var restaurants = await this.restaurantsRepository
                .All()
                .Where(r => r.DestinationId == destinationId)
                .To<RestaurantViewModel>()
                .ToArrayAsync();
            return restaurants;
        }

        public async Task<RestaurantDetailsViewModel> CreateAsync(RestaurantCreateInputModel restaurantCreateInputModel)
        {
            if (!Enum.TryParse(restaurantCreateInputModel.Type, true, out RestaurantType typeEnum))
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.InvalidRestaurantType, restaurantCreateInputModel.Type));
            }

            // If destination exists return existing view model
            var restaurantExists = this.restaurantsRepository.All().Any(
                r => r.Name == restaurantCreateInputModel.Name &&
                     r.DestinationId == restaurantCreateInputModel.DestinationId);
            if (restaurantExists)
            {
                return AutoMapper.Mapper
                    .Map<RestaurantDetailsViewModel>(this.restaurantsRepository.All()
                        .First(r => r.Name == restaurantCreateInputModel.Name &&
                                    r.DestinationId == restaurantCreateInputModel.DestinationId));
            }

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

            var restaurantDetailsViewModel = AutoMap.Mapper.Map<RestaurantDetailsViewModel>(restaurant);
            return restaurantDetailsViewModel;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var restaurant = await this.restaurantsRepository
                .All()
                .Where(d => d.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();
            if (restaurant == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceRestaurantId, id));
            }

            return restaurant;
        }

        public async Task EditAsync(RestaurantEditViewModel restaurantEditViewModel)
        {
            if (!Enum.TryParse(restaurantEditViewModel.Type, true, out RestaurantType restaurantTypeEnum))
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.InvalidRestaurantType, restaurantEditViewModel.Type));
            }

            var restaurant = this.restaurantsRepository.All().FirstOrDefault(r => r.Id == restaurantEditViewModel.Id);
            if (restaurant == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceRestaurantId, restaurantEditViewModel.Id));
            }

            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == restaurantEditViewModel.DestinationId);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, restaurantEditViewModel.DestinationId));
            }

            if (restaurantEditViewModel.NewImage != null)
            {
                var newImageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, restaurantEditViewModel.NewImage, restaurantEditViewModel.Name);
                restaurant.ImageUrl = newImageUrl;
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
            var restaurant = await this.restaurantsRepository
                .All()
                .FirstOrDefaultAsync(d => d.Id == id);
            if (restaurant == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceRestaurantId, id));
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
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var restaurant = await this.restaurantsRepository.All().Where(r => r.Id == restaurantId).FirstOrDefaultAsync();
            if (restaurant == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceRestaurantId, restaurantId));
            }

            if (this.restaurantReviewsRepository.All().Any(r => r.RestaurantId == restaurantId && r.Review.User == user))
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.RestaurantReviewAlreadyAdded, user.UserName, restaurant.Id, restaurant.Name));
            }

            var review = new Review
            {
                User = user,
                Rating = restaurantReviewInputModel.Rating,
                Content = restaurantReviewInputModel.Content,
            };

            this.reviewsRepository.Add(review);
            await this.reviewsRepository.SaveChangesAsync();

            var restaurantReview = new RestaurantReview
            {
                Restaurant = restaurant,
                Review = review,
            };

            this.restaurantReviewsRepository.Add(restaurantReview);
            await this.restaurantReviewsRepository.SaveChangesAsync();

            await this.UpdateRestaurantAverageRating(restaurant);
        }

        public IEnumerable<RestaurantViewModel> GetRestaurantsFromSearch(string searchString, int? destinationId)
        {
            var escapedSearchTokens = searchString.Split(new[] { ' ', ',', '.', ':', '=', ';' }, StringSplitOptions.RemoveEmptyEntries);

            var restaurants = this.restaurantsRepository
                .All()
                .Where(r => escapedSearchTokens.All(t => r.Name.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => r.Address.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => r.Destination.Name.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => r.Type.ToString().ToLower().Contains(t.ToLower())))
                .To<RestaurantViewModel>()
                .ToArray();

            return destinationId == null ? restaurants : restaurants.Where(r => r.DestinationId == destinationId);
        }

        public IEnumerable<RestaurantViewModel> SortBy(RestaurantViewModel[] restaurants, RestaurantSorter sorter)
        {
            switch (sorter)
            {
                case RestaurantSorter.Name:
                    return restaurants.OrderBy(d => d.Name).ToArray();
                case RestaurantSorter.Rating:
                    return restaurants.OrderByDescending(d => d.AverageRating).ToArray();
                case RestaurantSorter.Type:
                    return restaurants.OrderBy(d => d.Type).ToArray();
                case RestaurantSorter.Destination:
                    return restaurants.OrderBy(d => d.DestinationName).ToArray();
                default:
                    return restaurants.OrderBy(d => d.Name).ToArray();
            }
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

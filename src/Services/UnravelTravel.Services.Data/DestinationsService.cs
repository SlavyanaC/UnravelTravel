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
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Destinations;
    using UnravelTravel.Services.Data.Models.Home;
    using UnravelTravel.Services.Data.Utilities;
    using UnravelTravel.Services.Mapping;

    public class DestinationsService : IDestinationsService
    {
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<Country> countriesRepository;

        private readonly IActivitiesService activitiesService;
        private readonly IRestaurantsService restaurantsService;

        private readonly Cloudinary cloudinary;

        public DestinationsService(IRepository<Destination> destinationsRepository, IRepository<Country> countriesRepository, IActivitiesService activitiesService, IRestaurantsService restaurantsService, Cloudinary cloudinary)
        {
            this.destinationsRepository = destinationsRepository;
            this.countriesRepository = countriesRepository;

            this.activitiesService = activitiesService;
            this.restaurantsService = restaurantsService;

            this.cloudinary = cloudinary;
        }

        public async Task<DestinationViewModel[]> GetAllDestinationsAsync()
        {
            var destinations = await this.destinationsRepository
                .All()
                .To<DestinationViewModel>()
                .OrderBy(d => d.CountryName)
                .ThenBy(d => d.Name)
                .ToArrayAsync();

            return destinations;
        }

        public async Task<int> CreateAsync(params object[] parameters)
        {
            var name = parameters[0].ToString();
            var countryId = int.Parse(parameters[1].ToString());
            var image = parameters[2] as IFormFile;
            var information = parameters[3].ToString();

            var imageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, image, name);

            var destination = new Destination
            {
                Name = name,
                CountryId = countryId,
                ImageUrl = imageUrl,
                Information = information,
            };

            this.destinationsRepository.Add(destination);
            await this.destinationsRepository.SaveChangesAsync();

            return destination.Id;
        }

        public async Task<TViewModel> GetViewModelAsync<TViewModel>(int id)
        {
            var destination = await this.destinationsRepository
                .All()
                .Where(d => d.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            return destination;
        }

        public async Task EditAsync(int id, params object[] parameters)
        {
            var name = parameters[0].ToString();
            var countryId = int.Parse(parameters[1].ToString());
            var newImage = parameters[2] as IFormFile;
            var information = parameters[3].ToString();

            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == id);
            var country = this.countriesRepository.All().FirstOrDefault(c => c.Id == countryId);

            if (newImage != null)
            {
                var newImageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, newImage, name);
                destination.ImageUrl = newImageUrl;
            }

            destination.Name = name;
            destination.Country = country;
            destination.Information = information;

            this.destinationsRepository.Update(destination);
            await this.destinationsRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == id);
            destination.IsDeleted = true;

            this.destinationsRepository.Update(destination);
            await this.destinationsRepository.SaveChangesAsync();
        }

        public SearchResultViewModel GetSearchResult(int destinationId, DateTime startDate, DateTime endDate)
        {
            var activities = this.activitiesService.GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .Where(a => a.Location.Destination.Id == destinationId &&
                            a.Date >= startDate &&
                            a.Date <= endDate)
                .ToArray();

            var restaurants = this.restaurantsService.GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .Where(r => r.DestinationId == destinationId)
                .ToArray();

            var searchResultViewModel = new SearchResultViewModel
            {
                Activities = activities,
                Restaurants = restaurants,
            };

            return searchResultViewModel;
        }
    }
}

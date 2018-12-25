namespace UnravelTravel.Services.Data
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Models.ViewModels.Home;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
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
            var destinations = await this.destinationsRepository.All()
                .To<DestinationViewModel>()
                .OrderBy(d => d.CountryName)
                .ThenBy(d => d.Name)
                .ToArrayAsync();

            return destinations;
        }

        public async Task<int> CreateAsync(DestinationCreateInputModel destinationCreateInputModel)
        {
            var imageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, destinationCreateInputModel.Image, destinationCreateInputModel.Name);

            var destination = new Destination
            {
                Name = destinationCreateInputModel.Name,
                CountryId = destinationCreateInputModel.CountryId,
                ImageUrl = imageUrl,
                Information = destinationCreateInputModel.Information,
            };

            this.destinationsRepository.Add(destination);
            await this.destinationsRepository.SaveChangesAsync();

            return destination.Id;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var destination = await this.destinationsRepository.All()
                .Where(d => d.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            return destination;
        }

        public async Task EditAsync(DestinationEditViewModel destinationEditViewModel)
        {
            var destination = this.destinationsRepository.All()
                .FirstOrDefault(d => d.Id == destinationEditViewModel.Id);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, destinationEditViewModel.Id));
            }

            var country = this.countriesRepository.All()
                .FirstOrDefault(c => c.Id == destinationEditViewModel.CountryId);
            if (country == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCountryId, destinationEditViewModel.CountryId));
            }

            if (destinationEditViewModel.NewImage != null)
            {
                var newImageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, destinationEditViewModel.NewImage, destinationEditViewModel.Name);
                destination.ImageUrl = newImageUrl;
            }

            destination.Name = destinationEditViewModel.Name;
            destination.Country = country;
            destination.Information = destinationEditViewModel.Information;

            this.destinationsRepository.Update(destination);
            await this.destinationsRepository.SaveChangesAsync();

        }

        public async Task DeleteByIdAsync(int id)
        {
            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == id);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, id));
            }

            destination.IsDeleted = true;

            this.destinationsRepository.Update(destination);
            await this.destinationsRepository.SaveChangesAsync();
        }

        public async Task<SearchResultViewModel> GetSearchResultAsync(int destinationId, DateTime startDate, DateTime endDate)
        {
            var destination = await this.destinationsRepository.All()
                .FirstOrDefaultAsync(d => d.Id == destinationId);

            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, destinationId));
            }

            var activities = this.activitiesService.GetAllAsync()
                .GetAwaiter()
                .GetResult()
                .Where(a => a.LocationDestinationId == destinationId &&
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
                DestinationName = destination.Name,
                StartDate = startDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture),
                EndDate = endDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture),
                Activities = activities,
                Restaurants = restaurants,
            };

            return searchResultViewModel;
        }
    }
}

﻿namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Destinations;
    using UnravelTravel.Models.ViewModels.Destinations;
    using UnravelTravel.Models.ViewModels.Enums;
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

        public DestinationsService(
            IRepository<Destination> destinationsRepository,
            IRepository<Country> countriesRepository,
            IActivitiesService activitiesService,
            IRestaurantsService restaurantsService,
            Cloudinary cloudinary)
        {
            this.destinationsRepository = destinationsRepository;
            this.countriesRepository = countriesRepository;

            this.activitiesService = activitiesService;
            this.restaurantsService = restaurantsService;

            this.cloudinary = cloudinary;
        }

        public async Task<IEnumerable<DestinationViewModel>> GetAllDestinationsAsync()
        {
            var destinations = await this.destinationsRepository.All()
                .To<DestinationViewModel>()
                .OrderBy(d => d.CountryName)
                .ThenBy(d => d.Name)
                .ToArrayAsync();

            return destinations;
        }

        public async Task<DestinationDetailsViewModel> CreateAsync(DestinationCreateInputModel destinationCreateInputModel)
        {
            var country = await this.countriesRepository.All().FirstOrDefaultAsync(c => c.Id == destinationCreateInputModel.CountryId);
            if (country == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCountryId, destinationCreateInputModel.CountryId));
            }

            // If destination exists return existing view model
            var destinationExists = this.destinationsRepository.All().Any(d =>
                d.Name == destinationCreateInputModel.Name && d.CountryId == destinationCreateInputModel.CountryId);
            if (destinationExists)
            {
                return AutoMapper.Mapper
                    .Map<DestinationDetailsViewModel>(this.destinationsRepository.All()
                        .First(d => d.Name == destinationCreateInputModel.Name &&
                                    d.CountryId == destinationCreateInputModel.CountryId));
            }

            var imageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, destinationCreateInputModel.Image, destinationCreateInputModel.Name);

            var googleServiceInfo =
                DateTimeExtensions.GetGoogleServiceInfo(destinationCreateInputModel.Name, country.Name);

            var destination = new Destination
            {
                Name = destinationCreateInputModel.Name,
                CountryId = destinationCreateInputModel.CountryId,
                ImageUrl = imageUrl,
                Information = destinationCreateInputModel.Information,
                Latitude = googleServiceInfo.Latitude,
                Longitude = googleServiceInfo.Longitude,
                UtcRawOffset = googleServiceInfo.UtcRawOffset,
            };

            this.destinationsRepository.Add(destination);
            await this.destinationsRepository.SaveChangesAsync();

            var destinationDetailsViewModel = AutoMapper.Mapper.Map<DestinationDetailsViewModel>(destination);
            return destinationDetailsViewModel;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var destination = await this.destinationsRepository.All()
                .Where(d => d.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, id));
            }

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

            var activities = this.activitiesService.GetAllAsync().GetAwaiter().GetResult()
                .Where(a => a.DestinationId == destinationId &&
                            a.Date >= startDate &&
                            a.Date <= endDate)
                .OrderBy(a => a.Date)
                .ToArray();

            var restaurants = this.restaurantsService.GetAllAsync().GetAwaiter().GetResult()
                .Where(r => r.DestinationId == destinationId)
                .OrderByDescending(r => r.AverageRating)
                .ToArray();

            var searchResultViewModel = new SearchResultViewModel
            {
                DestinationId = destination.Id,
                DestinationName = destination.Name,
                StartDate = startDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture),
                EndDate = endDate.ToString(GlobalConstants.DateFormat, CultureInfo.InvariantCulture),
                Activities = activities,
                Restaurants = restaurants,
            };

            return searchResultViewModel;
        }

        public async Task<string> GetDestinationName(int destinationId)
        {
            var destination = await this.destinationsRepository.All().FirstOrDefaultAsync(d => d.Id == destinationId);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, destinationId));
            }

            return destination.Name;
        }

        public IEnumerable<DestinationViewModel> GetDestinationFromSearch(string searchString)
        {
            var escapedSearchTokens = searchString.Split(new char[] { ' ', ',', '.', ':', '=', ';' }, StringSplitOptions.RemoveEmptyEntries);

            var destinations = this.destinationsRepository
                .All()
                .Where(d => escapedSearchTokens.All(t => d.Name.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => d.Country.Name.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => d.Activities.Any(a =>
                                                            a.Name.ToLower().Contains(t.ToLower()))) ||
                            escapedSearchTokens.All(t => d.Restaurants.Any(a =>
                                                            a.Name.ToLower().Contains(t.ToLower()))))
                .To<DestinationViewModel>()
                .ToArray();

            return destinations;
        }

        public IEnumerable<DestinationViewModel> SortBy(DestinationViewModel[] destinations, DestinationSorter sorter)
        {
            switch (sorter)
            {
                case DestinationSorter.CountryName:
                    return destinations.OrderBy(d => d.CountryName).ThenBy(d => d.Name).ToArray();
                case DestinationSorter.ActivitiesCount:
                    return destinations.OrderByDescending(d => d.ActivitiesCount).ToArray();
                case DestinationSorter.RestaurantsCount:
                    return destinations.OrderByDescending(d => d.RestaurantsCount).ToArray();
                default:
                    return destinations.OrderBy(d => d.CountryName).ThenBy(d => d.Name).ToArray();
            }
        }
    }
}

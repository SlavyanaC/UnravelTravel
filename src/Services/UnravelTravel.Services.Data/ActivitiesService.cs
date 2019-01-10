﻿namespace UnravelTravel.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using GoogleMaps.LocationServices;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Activities;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Models.ViewModels.Enums;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public class ActivitiesService : IActivitiesService
    {
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<Review> reviewsRepository;
        private readonly IRepository<ActivityReview> activityReviewsRepository;
        private readonly IRepository<UnravelTravelUser> usersRepository;
        private readonly Cloudinary cloudinary;

        public ActivitiesService(
            IRepository<Activity> activitiesRepository,
            IRepository<Destination> destinationsRepository,
            Cloudinary cloudinary,
            IRepository<Review> reviewsRepository,
            IRepository<ActivityReview> activityReviewsRepository,
            IRepository<UnravelTravelUser> usersRepository)
        {
            this.activitiesRepository = activitiesRepository;
            this.destinationsRepository = destinationsRepository;
            this.cloudinary = cloudinary;
            this.reviewsRepository = reviewsRepository;
            this.activityReviewsRepository = activityReviewsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task<IEnumerable<ActivityViewModel>> GetAllAsync()
        {
            var activities = await this.activitiesRepository
                .All()
                .To<ActivityViewModel>()
                .ToArrayAsync();

            return activities;
        }

        public async Task<IEnumerable<ActivityViewModel>> GetAllInDestinationAsync(int destinationId)
        {
            var activities = await this.activitiesRepository
                .All()
                .Where(a => a.DestinationId == destinationId)
                .To<ActivityViewModel>()
                .ToArrayAsync();

            return activities;
        }

        public async Task<ActivityDetailsViewModel> CreateAsync(ActivityCreateInputModel activityCreateInputModel)
        {
            if (!Enum.TryParse(activityCreateInputModel.Type, true, out ActivityType activityTypeEnum))
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.InvalidActivityType, activityCreateInputModel.Type));
            }

            var destination = await this.destinationsRepository.All().FirstOrDefaultAsync(l => l.Id == activityCreateInputModel.DestinationId);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, activityCreateInputModel.DestinationId));
            }

            var imageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, activityCreateInputModel.Image, activityCreateInputModel.Name);

            // var utcDate = activityCreateInputModel.Date.GetUtcDate(destination.Name, destination.Country.Name);
            var utcDate = activityCreateInputModel.Date.CalculateUtcDateTime(destination.UtcRawOffset);

            var activity = new Activity
            {
                Name = activityCreateInputModel.Name,
                ImageUrl = imageUrl,
                Date = utcDate,
                Type = activityTypeEnum,
                Description = activityCreateInputModel.Description,
                AdditionalInfo = activityCreateInputModel.AdditionalInfo,
                Destination = destination,
                Address = activityCreateInputModel.Address,
                LocationName = activityCreateInputModel.LocationName,
                Price = activityCreateInputModel.Price,
            };

            this.activitiesRepository.Add(activity);
            await this.activitiesRepository.SaveChangesAsync();

            var activityDetailsViewModel = AutoMapper.Mapper.Map<ActivityDetailsViewModel>(activity);
            return activityDetailsViewModel;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var activity = await this.activitiesRepository
                .All()
                .Where(a => a.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, id));
            }

            return activity;
        }

        public async Task EditAsync(ActivityEditViewModel activityToEditViewModel)
        {
            if (!Enum.TryParse(activityToEditViewModel.Type, true, out ActivityType activityTypeEnum))
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.InvalidActivityType, activityToEditViewModel.Type));
            }

            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == activityToEditViewModel.Id);
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, activityToEditViewModel.Id));
            }

            var destination = await this.destinationsRepository.All().FirstOrDefaultAsync(l => l.Id == activityToEditViewModel.DestinationId);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, activityToEditViewModel.DestinationId));
            }

            if (activityToEditViewModel.NewImage != null)
            {
                var newImageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, activityToEditViewModel.NewImage, activityToEditViewModel.Name);
                activity.ImageUrl = newImageUrl;
            }

            activity.Name = activityToEditViewModel.Name;
            activity.Type = activityTypeEnum;
            activity.Date = activityToEditViewModel.Date;
            activity.Description = activityToEditViewModel.Description;
            activity.AdditionalInfo = activityToEditViewModel.AdditionalInfo;
            activity.Destination = destination;
            activity.Address = activityToEditViewModel.Address;
            activity.LocationName = activityToEditViewModel.LocationName;
            activity.Price = activityToEditViewModel.Price;

            this.activitiesRepository.Update(activity);
            await this.activitiesRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var activity = this.activitiesRepository.All().FirstOrDefault(d => d.Id == id);
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, id));
            }

            activity.IsDeleted = true;
            this.activitiesRepository.Update(activity);
            await this.activitiesRepository.SaveChangesAsync();
        }

        public async Task Review(int activityId, string username, ActivityReviewInputModel activityReviewInputModel)
        {
            var user = await this.usersRepository.All().Where(u => u.UserName == username).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == activityId);
            if (activity == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceActivityId, activityId));
            }

            if (this.activityReviewsRepository.All().Any(r => r.ActivityId == activityId && r.Review.User == user))
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.ActivityReviewAlreadyAdded, user.UserName, activity.Id, activity.Name));
            }

            var review = new Review
            {
                User = user,
                Rating = activityReviewInputModel.Rating,
                Content = activityReviewInputModel.Content,
            };

            this.reviewsRepository.Add(review);
            await this.reviewsRepository.SaveChangesAsync();

            var activityReview = new ActivityReview
            {
                Activity = activity,
                Review = review,
            };

            this.activityReviewsRepository.Add(activityReview);
            await this.activityReviewsRepository.SaveChangesAsync();

            await this.UpdateActivityAverageRating(activity);
        }

        public IEnumerable<ActivityViewModel> GetActivitiesFromSearch(string searchString, int? destinationId)
        {
            var escapedSearchTokens = searchString.Split(new[] { ' ', ',', '.', ':', '=', ';' }, StringSplitOptions.RemoveEmptyEntries);

            var activities = this.activitiesRepository
                .All()
                .Where(a => escapedSearchTokens.All(t => a.Name.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => a.Description.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => a.Address.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => a.Destination.Name.ToLower().Contains(t.ToLower())) ||
                            escapedSearchTokens.All(t => a.Type.ToString().ToLower().Contains(t.ToLower())))
                .To<ActivityViewModel>()
                .ToArray();

            return destinationId == null ? activities : activities.Where(a => a.DestinationId == destinationId);
        }

        public IEnumerable<ActivityViewModel> SortBy(ActivityViewModel[] activities, ActivitiesSorter sorter)
        {
            ActivityViewModel[] passedActivities;
            switch (sorter)
            {
                case ActivitiesSorter.Upcoming:
                    passedActivities = activities.OrderBy(a => a.Date).Where(a => a.Date < DateTime.UtcNow).ToArray();
                    break;
                case ActivitiesSorter.Name:
                    passedActivities = activities.OrderBy(a => a.Name).Where(a => a.Date < DateTime.UtcNow).ToArray();
                    break;
                case ActivitiesSorter.Type:
                    passedActivities = activities.OrderBy(a => a.Type).Where(a => a.Date < DateTime.UtcNow).ToArray();
                    break;
                case ActivitiesSorter.Destination:
                    passedActivities = activities.OrderBy(a => a.DestinationName).Where(a => a.Date < DateTime.UtcNow).ToArray();
                    break;
                default:
                    passedActivities = activities.OrderBy(a => a.Date).Where(a => a.Date < DateTime.UtcNow).ToArray();
                    break;
            }

            switch (sorter)
            {
                case ActivitiesSorter.Upcoming:
                    return activities.OrderBy(a => a.Date).Where(a => a.Date >= DateTime.UtcNow).ToArray().Concat(passedActivities);
                case ActivitiesSorter.Name:
                    return activities.OrderBy(a => a.Name).Where(a => a.Date >= DateTime.UtcNow).ToArray().Concat(passedActivities);
                case ActivitiesSorter.Type:
                    return activities.OrderBy(a => a.Type).Where(a => a.Date >= DateTime.UtcNow).ToArray().Concat(passedActivities);
                case ActivitiesSorter.Destination:
                    return activities.OrderBy(a => a.DestinationName).Where(a => a.Date >= DateTime.UtcNow).ToArray().Concat(passedActivities);
                default:
                    return activities.OrderBy(a => a.Date).Where(a => a.Date >= DateTime.UtcNow).ToArray().Concat(passedActivities);
            }
        }

        private async Task UpdateActivityAverageRating(Activity activity)
        {
            var avgRating = activity.Reviews.Average(ar => ar.Review.Rating);
            activity.AverageRating = avgRating;

            this.activitiesRepository.Update(activity);
            await this.activitiesRepository.SaveChangesAsync();
        }
    }
}

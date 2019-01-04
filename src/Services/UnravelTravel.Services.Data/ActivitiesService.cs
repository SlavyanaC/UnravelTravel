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
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Activities;
    using UnravelTravel.Models.InputModels.Reviews;
    using UnravelTravel.Models.ViewModels.Activities;
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

        public async Task<ActivityViewModel[]> GetAllAsync()
        {
            var activities = await this.activitiesRepository
                .All()
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

            var activity = new Activity
            {
                Name = activityCreateInputModel.Name,
                ImageUrl = imageUrl,
                Date = activityCreateInputModel.Date,
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

        private async Task UpdateActivityAverageRating(Activity activity)
        {
            var avgRating = activity.Reviews.Average(ar => ar.Review.Rating);
            activity.AverageRating = avgRating;

            this.activitiesRepository.Update(activity);
            await this.activitiesRepository.SaveChangesAsync();
        }
    }
}

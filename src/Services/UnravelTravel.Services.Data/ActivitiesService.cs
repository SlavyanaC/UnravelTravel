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
    using UnravelTravel.Services.Data.Models.Activities;
    using UnravelTravel.Services.Data.Utilities;
    using UnravelTravel.Services.Mapping;

    public class ActivitiesService : IActivitiesService
    {
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IRepository<Location> locationsRepository;
        private readonly IRepository<Review> reviewsRepository;
        private readonly IRepository<ActivityReview> activityReviewsRepository;
        private readonly IRepository<ApplicationUser> usersRepository;
        private readonly Cloudinary cloudinary;

        public ActivitiesService(IRepository<Activity> activitiesRepository, IRepository<Location> locationsRepository, Cloudinary cloudinary, IRepository<Review> reviewsRepository, IRepository<ActivityReview> activityReviewsRepository, IRepository<ApplicationUser> usersRepository)
        {
            this.activitiesRepository = activitiesRepository;
            this.locationsRepository = locationsRepository;
            this.cloudinary = cloudinary;
            this.reviewsRepository = reviewsRepository;
            this.activityReviewsRepository = activityReviewsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task<ActivityViewModel[]> GetAllActivitiesAsync()
        {
            var activities = await this.activitiesRepository
                .All()
                .To<ActivityViewModel>()
                .ToArrayAsync();

            return activities;
        }

        public async Task<int> CreateAsync(params object[] parameters)
        {
            var name = parameters[0].ToString();
            var image = parameters[1] as IFormFile;
            var date = DateTime.Parse(parameters[2].ToString());
            var typeString = parameters[3].ToString();
            var locationId = int.Parse(parameters[4].ToString());

            var imageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, image, name);

            Enum.TryParse(typeString, true, out ActivityType typeEnum);

            var location = await this.locationsRepository
                .All()
                .FirstOrDefaultAsync(l => l.Id == locationId);

            var activity = new Activity
            {
                Name = name,
                ImageUrl = imageUrl,
                Date = date,
                Type = typeEnum,
                Location = location,
            };

            this.activitiesRepository.Add(activity);
            await this.activitiesRepository.SaveChangesAsync();

            return activity.Id;
        }

        public async Task<TViewModel> GetViewModelAsync<TViewModel>(int id)
        {
            var activity = await this.activitiesRepository
                .All()
                .Where(a => a.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            return activity;
        }

        public async Task EditAsync(int id, params object[] parameters)
        {
            var name = parameters[0].ToString();
            var newImage = parameters[1] as IFormFile;
            var date = DateTime.Parse(parameters[2].ToString());
            var typeString = parameters[3].ToString();
            var locationId = int.Parse(parameters[4].ToString());

            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == id);
            var location = await this.locationsRepository.All().FirstOrDefaultAsync(l => l.Id == locationId);

            Enum.TryParse(typeString, true, out ActivityType typeEnum);

            if (newImage != null)
            {
                var newImageUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, newImage, name);
                activity.ImageUrl = newImageUrl;
            }

            activity.Name = name;
            activity.Type = typeEnum;
            activity.Date = date;
            activity.Location = location;

            this.activitiesRepository.Update(activity);
            await this.activitiesRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var destination = this.activitiesRepository.All().FirstOrDefault(d => d.Id == id);
            destination.IsDeleted = true;

            this.activitiesRepository.Update(destination);
            await this.activitiesRepository.SaveChangesAsync();
        }

        public async Task Review(int id, string username, params object[] parameters)
        {
            var rating = double.Parse(parameters[0].ToString());
            var content = parameters[1].ToString();

            var user = await this.usersRepository.All().Where(u => u.UserName == username).FirstOrDefaultAsync();
            var review = new Review
            {
                Rating = rating,
                Content = content,
                User = user,
            };

            this.reviewsRepository.Add(review);
            await this.reviewsRepository.SaveChangesAsync();

            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == id);

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

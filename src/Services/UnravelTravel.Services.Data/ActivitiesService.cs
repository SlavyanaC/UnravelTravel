namespace UnravelTravel.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Activities;

    public class ActivitiesService : IActivitiesService
    {
        private readonly IRepository<Activity> activitiesRepository;
        private readonly IRepository<Location> locationsRepository;

        public ActivitiesService(IRepository<Activity> activitiesRepository, IRepository<Location> locationsRepository)
        {
            this.activitiesRepository = activitiesRepository;
            this.locationsRepository = locationsRepository;
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
            var date = DateTime.Parse(parameters[1].ToString());
            var typeString = parameters[2].ToString();

            Enum.TryParse(typeString, true, out ActivityType typeEnum);

            var activity = new Activity
            {
                Name = name,
                Date = date,
                Type = typeEnum,
            };

            var locationString = parameters[3].ToString();
            var location = await this.locationsRepository
                .All()
                .FirstOrDefaultAsync(l => l.Name == locationString);

            if (location == null)
            {
                // TODO: Some logic here;
            }

            activity.Location = location;

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
            var typeString = parameters[1].ToString();
            var date = DateTime.Parse(parameters[2].ToString());
            var locationId = int.Parse(parameters[3].ToString());
            var locationName = parameters[4].ToString();

            var activity = await this.activitiesRepository.All().FirstOrDefaultAsync(a => a.Id == id);
            var location = await this.locationsRepository.All().FirstOrDefaultAsync(l => l.Id == locationId || l.Name == locationName);

            Enum.TryParse(typeString, true, out ActivityType typeEnum);

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

            this.activitiesRepository.Delete(destination);
            await this.activitiesRepository.SaveChangesAsync();
        }
    }
}

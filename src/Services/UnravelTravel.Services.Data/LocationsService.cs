
namespace UnravelTravel.Services.Data
{
    using System;
    using System.Threading.Tasks;
    using AutoMap = AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.InputModels.AdministratorInputModels.Locations;
    using UnravelTravel.Models.ViewModels.Locations;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public class LocationsService : ILocationsService
    {
        private readonly IRepository<Location> locationsRepository;
        private readonly IRepository<Destination> destinationsRepository;

        public LocationsService(IRepository<Location> locationsRepository, IRepository<Destination> destinationsRepository)
        {
            this.locationsRepository = locationsRepository;
            this.destinationsRepository = destinationsRepository;
        }

        public async Task<LocationViewModel> CreateLocationAsync(LocationCreateInputModel locationCreateInputModel)
        {
            var destination = await this.destinationsRepository.All().FirstOrDefaultAsync(d => d.Id == locationCreateInputModel.DestinationId);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, locationCreateInputModel.DestinationId));
            }

            var typeString = locationCreateInputModel.Type;
            if (!Enum.TryParse(typeString, true, out LocationType typeEnum))
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.InvalidLocationType, typeString));
            }

            var location = new Location
            {
                Name = locationCreateInputModel.Name,
                Address = locationCreateInputModel.Address,
                Destination = destination,
                LocationType = typeEnum,
            };

            this.locationsRepository.Add(location);
            await this.locationsRepository.SaveChangesAsync();

            return AutoMap.Mapper.Map<LocationViewModel>(location);
        }

        public async Task<LocationViewModel[]> GetAllLocationsAsync()
        {
            var locations = await this.locationsRepository
                .All()
                .To<LocationViewModel>()
                .ToArrayAsync();

            return locations;
        }
    }
}

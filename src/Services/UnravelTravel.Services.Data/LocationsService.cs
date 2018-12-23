namespace UnravelTravel.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Models.Enums;
    using UnravelTravel.Models.ViewModels.Locations;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Utilities;
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

        public async Task<int> CreateAsync(params object[] parameters)
        {
            var name = parameters[0].ToString();
            var address = parameters[1].ToString();

            var destinationId = int.Parse(parameters[2].ToString());
            var destination = await this.destinationsRepository.All().FirstOrDefaultAsync(d => d.Id == destinationId);
            if (destination == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceDestinationId, destinationId));
            }

            var typeString = parameters[3].ToString();
            Enum.TryParse(typeString, true, out LocationType typeEnum);

            var location = new Location
            {
                Name = name,
                Address = address,
                Destination = destination,
                LocationType = typeEnum,
            };

            this.locationsRepository.Add(location);
            await this.locationsRepository.SaveChangesAsync();

            return location.Id;
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

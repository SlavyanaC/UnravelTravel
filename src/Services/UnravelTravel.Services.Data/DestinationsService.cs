namespace UnravelTravel.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Destinations;

    public class DestinationsService : IDestinationsService
    {
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<Country> countriesRepository;

        public DestinationsService(IRepository<Destination> destinationsRepository, IRepository<Country> countriesRepository)
        {
            this.destinationsRepository = destinationsRepository;
            this.countriesRepository = countriesRepository;
        }

        public async Task<int> CreateAsync(string name, int countryId, string imageUrl, string information)
        {
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

        public async Task<DestinationDetailsViewModel> GetDestinationDetailsAsync(int id)
        {
            var destination = await this.destinationsRepository
                .All()
                .Where(d => d.Id == id)
                .To<DestinationDetailsViewModel>()
                .FirstOrDefaultAsync();

            return destination;
        }

        public async Task<AllDestinationsViewModel> GetAllDestinationsAsync()
        {
            var destinations = await this.destinationsRepository
                .All()
                .To<DestinationViewModel>()
                .ToListAsync();

            var allDestinations = new AllDestinationsViewModel
            {
                Destination = destinations,
            };

            return allDestinations;
        }

        public async Task<DestinationEditViewModel> GetDestinationToEditAsync(int id)
        {
            var destination = await this.destinationsRepository
                .All()
                .Where(d => d.Id == id)
                .To<DestinationEditViewModel>()
                .FirstOrDefaultAsync();

            return destination;
        }

        public async Task EditDestinationAsync(int id, string name, int countryId, string imageUrl, string information)
        {
            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == id);
            var country = this.countriesRepository.All().FirstOrDefault(c => c.Id == countryId);

            destination.Name = name;
            destination.Country = country;
            destination.ImageUrl = imageUrl;
            destination.Information = information;

            this.destinationsRepository.Update(destination);
            await this.destinationsRepository.SaveChangesAsync();
        }

        public async Task DeleteDestination(int id)
        {
            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == id);

            this.destinationsRepository.Delete(destination);
            await this.destinationsRepository.SaveChangesAsync();
        }
    }
}

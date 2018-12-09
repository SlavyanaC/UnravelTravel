namespace UnravelTravel.Services.Data
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Destinations;
    using UnravelTravel.Services.Mapping;

    public class DestinationsService : IDestinationsService
    {
        private readonly IRepository<Destination> destinationsRepository;
        private readonly IRepository<Country> countriesRepository;

        public DestinationsService(IRepository<Destination> destinationsRepository, IRepository<Country> countriesRepository)
        {
            this.destinationsRepository = destinationsRepository;
            this.countriesRepository = countriesRepository;
        }

        public async Task<DestinationViewModel[]> GetAllDestinationsAsync()
        {
            var destinations = await this.destinationsRepository
                .All()
                .To<DestinationViewModel>()
                .ToArrayAsync();

            return destinations;
        }

        public async Task<int> CreateAsync(params object[] parameters)
        {
            var name = parameters[0].ToString();
            var countryId = int.Parse(parameters[1].ToString());
            var imageUrl = parameters[2].ToString();
            var information = parameters[3].ToString();

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
            var imageUrl = parameters[2].ToString();
            var information = parameters[3].ToString();

            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == id);
            var country = this.countriesRepository.All().FirstOrDefault(c => c.Id == countryId);

            destination.Name = name;
            destination.Country = country;
            destination.ImageUrl = imageUrl;
            destination.Information = information;

            this.destinationsRepository.Update(destination);
            await this.destinationsRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var destination = this.destinationsRepository.All().FirstOrDefault(d => d.Id == id);

            this.destinationsRepository.Delete(destination);
            await this.destinationsRepository.SaveChangesAsync();
        }
    }
}

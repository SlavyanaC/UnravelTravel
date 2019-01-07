namespace UnravelTravel.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Countries;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        public async Task<IEnumerable<CountryViewModel>> GetAllAsync()
        {
            var countries = await this.countriesRepository.All()
                .OrderBy(c => c.Name)
                .To<CountryViewModel>()
                .ToArrayAsync();

            return countries;
        }
    }
}

namespace UnravelTravel.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using UnravelTravel.Common.Mapping;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Data.Models.Countries;

    public class CountriesService : ICountriesService
    {
        private readonly IRepository<Country> countriesRepository;

        public CountriesService(IRepository<Country> countriesRepository)
        {
            this.countriesRepository = countriesRepository;
        }

        IEnumerable<CountryViewModel> ICountriesService.GetAll()
        {
            var countries = this.countriesRepository
                .All()
                .OrderBy(c => c.Name)
                .To<CountryViewModel>()
                .ToList();

            return countries;
        }
    }
}

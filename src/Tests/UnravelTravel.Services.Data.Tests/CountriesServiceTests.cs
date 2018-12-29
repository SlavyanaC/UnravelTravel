using System.Reflection;
using AutoMapper;
using UnravelTravel.Models.InputModels.Account;
namespace UnravelTravel.Services.Data.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Countries;
    using UnravelTravel.Services.Data.Contracts;
    using Xunit;

    public class CountriesServiceTests : BaseServiceTests
    {
        private const string Bulgaria = "Bulgaria";
        private const string England = "England";
        private const string Netherlands = "Netherlands";

        private readonly ICountriesService countriesServiceMock;

        public CountriesServiceTests()
        {
            this.countriesServiceMock = this.Provider.GetRequiredService<ICountriesService>();
        }

        [Fact]
        public async Task GetAllAsyncReturnsAllCountries()
        {
            this.Context.Countries.Add(new Country { Id = 1, Name = Bulgaria });
            this.Context.Countries.Add(new Country { Id = 2, Name = England });
            this.Context.Countries.Add(new Country { Id = 3, Name = Netherlands });
            await this.Context.SaveChangesAsync();

            var expected = new CountryViewModel[]
            {
                new CountryViewModel {Id = 1, Name = Bulgaria},
                new CountryViewModel {Id = 2, Name = England},
                new CountryViewModel {Id = 3, Name = Netherlands},
            };

            var actual = await this.countriesServiceMock.GetAllAsync();

            Assert.Collection(actual,
                elem1 =>
                {
                    Assert.Equal(expected[0].Id, elem1.Id);
                    Assert.Equal(expected[0].Name, elem1.Name);
                },
                elem2 =>
                {
                    Assert.Equal(expected[1].Id, elem2.Id);
                    Assert.Equal(expected[1].Name, elem2.Name);
                },
                elem3 =>
                {
                    Assert.Equal(expected[2].Id, elem3.Id);
                    Assert.Equal(expected[2].Name, elem3.Name);
                });
            Assert.Equal(expected.Length, actual.Length);
        }
    }
}

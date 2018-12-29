namespace UnravelTravel.Services.Data.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.Countries;
    using UnravelTravel.Services.Data.Contracts;

    public class CountriesServiceTests : BaseServiceTests
    {
        private const string Bulgaria = "Bulgaria";
        private const string England = "England";
        private const string Netherlands = "Netherlands";

        private ICountriesService CountriesServiceMock => this.ServiceProvider.GetRequiredService<ICountriesService>();
       
        [Fact]
        public async Task GetAllAsyncReturnsAllCountries()
        {
            this.DbContext.Countries.Add(new Country { Id = 1, Name = Bulgaria });
            this.DbContext.Countries.Add(new Country { Id = 2, Name = England });
            this.DbContext.Countries.Add(new Country { Id = 3, Name = Netherlands });
            await this.DbContext.SaveChangesAsync();

            var expected = new CountryViewModel[]
            {
                new CountryViewModel {Id = 1, Name = Bulgaria},
                new CountryViewModel {Id = 2, Name = England},
                new CountryViewModel {Id = 3, Name = Netherlands},
            };

            var actual = await this.CountriesServiceMock.GetAllAsync();

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

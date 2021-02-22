using Microsoft.EntityFrameworkCore.Internal;

namespace UnravelTravel.Web.Middlewares
{
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using UnravelTravel.Data;
    using UnravelTravel.Data.Models;

    public class SeedCountriesMiddleware
    {
        private readonly RequestDelegate next;
        private UnravelTravelDbContext dbContext;

        public SeedCountriesMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            UnravelTravelDbContext dbContext)
        {
            this.dbContext = dbContext;

            await this.SeedCountries();
            await this.next(context);
        }

        private async Task SeedCountries()
        {
            var seededCountries = this.dbContext.Countries;
            if (seededCountries.Any())
            {
                return;
            }

            var jsonString = File.ReadAllText("countries.json");
            var countries = JsonConvert.DeserializeObject<Country[]>(jsonString);
            await this.dbContext.Countries.AddRangeAsync(countries);
            await this.dbContext.SaveChangesAsync();
        }
    }
}

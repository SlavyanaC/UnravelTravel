namespace Sandbox
{
    using System;
    using System.IO;
    using System.Text;
    using RestSharp;
    using GoogleMaps.LocationServices;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using UnravelTravel.Common;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Repositories;

    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"{typeof(Program).Namespace} ({string.Join(" ", args)}) starts working...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider(true);

            using (var serviceScope = serviceProvider.CreateScope())
            {
                serviceProvider = serviceScope.ServiceProvider;
                SandboxCode(serviceProvider);
            }
        }

        private static void SandboxCode(IServiceProvider serviceProvider)
        {
            //var context = serviceProvider.GetService<UnravelTravelDbContext>();
            //var jsonString = File.ReadAllText("countries.json");
            //var countries = JsonConvert.DeserializeObject<Country[]>(jsonString);
            //context.Countries.AddRange(countries);
            //context.SaveChanges();

            TestTimeZones();
        }

        private static void TestTimeZones()
        {
            var address = "Kaspitchan, Bulgaria";
            var locationService = new GoogleLocationService(apikey: GoogleUtilitiess.ApiKey);
            var point = locationService.GetLatLongFromAddress(address);
            var latitude = point.Latitude;
            var longitude = point.Longitude;

            var client = new RestClient(GoogleUtilitiess.BaseUrl);
            var requestTime = new RestRequest(GoogleUtilitiess.TimeZoneResource, Method.GET);
            requestTime.AddParameter("location", $"{latitude},{longitude}");
            requestTime.AddParameter("timestamp", GoogleUtilitiess.TimeStamp);
            requestTime.AddParameter("key", GoogleUtilitiess.ApiKey);
            var responseTime = client.Execute<GoogleTimeZone>(requestTime);
            var rawOffsetInSeconds = responseTime.Data.RawOffset;

            var dateHere = DateTime.Now;
            var utcDate = dateHere.Subtract(new TimeSpan(0, 0, (int)rawOffsetInSeconds));

            Console.WriteLine("Kaspitchan, Bulgaria - " + dateHere);
            Console.WriteLine("UCT -" + utcDate);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddDbContext<UnravelTravelDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        }
    }
}
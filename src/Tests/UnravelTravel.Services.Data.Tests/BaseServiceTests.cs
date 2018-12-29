namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Repositories;
    using UnravelTravel.Models.InputModels.Account;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public abstract class BaseServiceTests : IDisposable
    {
        protected BaseServiceTests()
        {
            var services = this.SetServices();

            this.ServiceProvider = services.BuildServiceProvider();
            this.DbContext = this.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        protected IServiceProvider ServiceProvider { get; set; }

        protected ApplicationDbContext DbContext { get; set; }

        private ServiceCollection SetServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(
                opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services
                    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequiredLength = 6;
                    })
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddUserStore<ApplicationUserStore>()
                    .AddRoleStore<ApplicationRoleStore>()
                    .AddDefaultTokenProviders();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // Application services
            services.AddScoped<IDestinationsService, DestinationsService>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IRestaurantsService, RestaurantsService>();
            services.AddScoped<IActivitiesService, ActivitiesService>();
            services.AddScoped<ILocationsService, LocationsService>();
            services.AddScoped<ITicketsService, TicketsService>();
            services.AddScoped<IReservationsService, ReservationsService>();
            services.AddScoped<IShoppingCartsService, ShoppingCartsService>();

            // Identity stores
            services.AddTransient<IUserStore<ApplicationUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();

            // AutoMapper
            AutoMapperConfig.RegisterMappings(typeof(LoginInputModel).GetTypeInfo().Assembly);

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>( new HttpContextAccessor {HttpContext = context });

            return services;
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            this.SetServices();
        }
    }
}
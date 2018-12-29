namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using AutoMapper;
    using UnravelTravel.Data;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Repositories;
    using UnravelTravel.Models.InputModels.Account;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;

    public abstract class BaseServiceTests : IDisposable
    {
        protected IServiceProvider Provider { get; set; }

        protected ApplicationDbContext Context { get; set; }

        protected BaseServiceTests()
        {
            var services = this.SetServices();

            this.Provider = services.BuildServiceProvider();
            this.Context = this.Provider.GetRequiredService<ApplicationDbContext>();
        }

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

            // TODO: AutoMapper doesn't work correctly here
            //Mapper.Reset();
            //AutoMapperConfig.RegisterMappings(typeof(LoginInputModel).GetTypeInfo().Assembly);

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>( new HttpContextAccessor {HttpContext = context });

            return services;
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            this.SetServices();
        }
    }
}
namespace UnravelTravel.Services.Data.Tests
{
    using System;
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using CloudinaryDotNet;
    using UnravelTravel.Data;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Repositories;
    using UnravelTravel.Models.ViewModels.Activities;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;
    using UnravelTravel.Services.Messaging;

    public abstract class BaseServiceTests : IDisposable
    {
        protected BaseServiceTests()
        {
            var services = this.SetServices();

            this.ServiceProvider = services.BuildServiceProvider();
            this.DbContext = this.ServiceProvider.GetRequiredService<UnravelTravelDbContext>();
        }

        protected IServiceProvider ServiceProvider { get; set; }

        protected UnravelTravelDbContext DbContext { get; set; }

        private ServiceCollection SetServices()
        {
            var services = new ServiceCollection();

            services.AddDbContext<UnravelTravelDbContext>(
                opt => opt.UseInMemoryDatabase(Guid.NewGuid().ToString()));

            services
                    .AddIdentity<UnravelTravelUser, ApplicationRole>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequiredLength = 6;
                    })
                    .AddEntityFrameworkStores<UnravelTravelDbContext>()
                    .AddUserStore<ApplicationUserStore>()
                    .AddRoleStore<ApplicationRoleStore>()
                    .AddDefaultTokenProviders();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // Application services
            services.AddTransient<IEmailSender, SendGridEmailSender>(provider =>
                new SendGridEmailSender(new LoggerFactory(), "SendGridKey", "unravelTravel@unravelTravel.com", "UnravelTravel"));

            services.AddScoped<IDestinationsService, DestinationsService>();
            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IRestaurantsService, RestaurantsService>();
            services.AddScoped<IActivitiesService, ActivitiesService>();
            services.AddScoped<ITicketsService, TicketsService>();
            services.AddScoped<IReservationsService, ReservationsService>();
            services.AddScoped<IShoppingCartsService, ShoppingCartsService>();

            // Identity stores
            services.AddTransient<IUserStore<UnravelTravelUser>, ApplicationUserStore>();
            services.AddTransient<IRoleStore<ApplicationRole>, ApplicationRoleStore>();

            // AutoMapper
            AutoMapperConfig.RegisterMappings(typeof(ActivityViewModel).GetTypeInfo().Assembly);

            // Cloudinary Setup
            var cloudinaryAccount = new CloudinaryDotNet.Account(CloudinaryConfig.CloudName, CloudinaryConfig.ApiKey, CloudinaryConfig.ApiSecret);
            var cloudinary = new Cloudinary(cloudinaryAccount);
            services.AddSingleton(cloudinary);

            var context = new DefaultHttpContext();
            services.AddSingleton<IHttpContextAccessor>(new HttpContextAccessor { HttpContext = context });

            return services;
        }

        public void Dispose()
        {
            DbContext.Database.EnsureDeleted();
            this.SetServices();
        }
    }
}
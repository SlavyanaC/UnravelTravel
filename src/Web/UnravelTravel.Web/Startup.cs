namespace UnravelTravel.Web
{
    using System;
    using System.Reflection;

    using CloudinaryDotNet;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Stripe;
    using UnravelTravel.Data;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Data.Repositories;
    using UnravelTravel.Data.Seeding;
    using UnravelTravel.Services.Data;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Services.Mapping;
    using UnravelTravel.Services.Messaging;
    using UnravelTravel.Web.Areas.Identity.Pages.Account.InputModels;
    using UnravelTravel.Web.Common;
    using UnravelTravel.Web.Middlewares;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Framework services
            // TODO: Add pooling when this bug is fixed: https://github.com/aspnet/EntityFrameworkCore/issues/9741
            services.AddDbContext<UnravelTravelDbContext>(options =>
            {
                options.UseLazyLoadingProxies();
                options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddDistributedSqlServerCache(
                options =>
                {
                    options.ConnectionString = this.configuration.GetConnectionString("DefaultConnection");
                    options.SchemaName = "dbo";
                    options.TableName = "CacheData";
                });

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.IdleTimeout = new TimeSpan(0, 4, 0, 0);
            });

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

            services.AddMvc(options =>
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddRazorPagesOptions(options =>
                {
                    options.AllowAreas = true;
                    options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                    options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
                });

            services.AddResponseCompression(options => { options.EnableForHttps = true; });

            services
                 .ConfigureApplicationCookie(options =>
                 {
                     options.LoginPath = "/Identity/Account/Login";
                     options.LogoutPath = "/Identity/Account/Logout";
                     options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                 });

            services
                .Configure<CookiePolicyOptions>(options =>
                {
                    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                    options.CheckConsentNeeded = context => true;
                    options.MinimumSameSitePolicy = SameSiteMode.Lax;
                    options.ConsentCookie.Name = ".AspNetCore.ConsentCookie";
                });

            services.AddSingleton(this.configuration);

            services.AddSession();

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

            // Application services
            services.AddTransient<IEmailSender, SendGridEmailSender>(provider =>
                new SendGridEmailSender(new LoggerFactory(), this.configuration["SendGridKey"], "unravelTravel@unravelTravel.com", "UnravelTravel"));

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

            // Facebook authentication
            services.AddAuthentication().AddFacebook(facebookOptions =>
            {
                facebookOptions.AppId = this.configuration["Authentication:Facebook:AppId"];
                facebookOptions.AppSecret = this.configuration["Authentication:Facebook:AppSecret"];
            });

            // Cloudinary Setup
            var cloudinaryAccount = new CloudinaryDotNet.Account(CloudinaryConfig.CloudName, CloudinaryConfig.ApiKey, CloudinaryConfig.ApiSecret);
            var cloudinary = new Cloudinary(cloudinaryAccount);
            services.AddSingleton(cloudinary);

            // Stripe Setup
            services.Configure<StripeSettings>(this.configuration.GetSection("Stripe"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            AutoMapperConfig.RegisterMappings(typeof(LoginInputModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<UnravelTravelDbContext>();

                if (env.IsDevelopment())
                {
                    dbContext.Database.Migrate();
                }

                ApplicationDbContextSeeder.Seed(dbContext, serviceScope.ServiceProvider);
            }

            loggerFactory.AddConsole(this.configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            // Configure Stripe ApiKeys
            StripeConfiguration.SetApiKey(this.configuration.GetSection("Stripe")["SecretKey"]);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseSession();
            app.UseSetAdminMiddleware();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

namespace UnravelTravel.Web.Middlewares
{
    using Microsoft.AspNetCore.Builder;

    public static class SeedCountriesMiddlewareExtensions
    {
        public static IApplicationBuilder UseSeedCountriesMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SeedCountriesMiddleware>();
        }
    }
}

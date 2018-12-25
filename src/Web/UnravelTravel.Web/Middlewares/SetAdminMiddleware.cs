using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UnravelTravel.Web.Middlewares
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using UnravelTravel.Common;
    using UnravelTravel.Data;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Services.Data.Contracts;

    public class SetAdminMiddleware
    {
        private readonly RequestDelegate next;

        public SetAdminMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext dbContext,
            IShoppingCartsService shoppingCartsService)
        {
            await SeedUserInRoles(userManager, shoppingCartsService);
            await this.next(context);
        }

        private static async Task SeedUserInRoles(UserManager<ApplicationUser> userManager, IShoppingCartsService shoppingCartsService)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser
                {
                    UserName = GlobalConstants.AdministratorUsername,
                    Email = GlobalConstants.AdministratorEmail,
                    FullName = GlobalConstants.AdministratorFullName,
                    ShoppingCart = new ShoppingCart(),
                };

                var result = await userManager.CreateAsync(user, GlobalConstants.AdministratorPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.AdministratorRoleName);
                    await shoppingCartsService.AssignShoppingCartsUserId(user);
                }
            }
        }
    }
}

namespace UnravelTravel.Data
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using UnravelTravel.Data.Models;

    public class ApplicationRoleStore : RoleStore<ApplicationRole,  UnravelTravelDbContext, string, IdentityUserRole<string>, IdentityRoleClaim<string>>
    {
        public ApplicationRoleStore(UnravelTravelDbContext context, IdentityErrorDescriber describer = null)
            : base(context, describer)
        {
        }

        protected override IdentityRoleClaim<string> CreateRoleClaim(ApplicationRole role, Claim claim) => new IdentityRoleClaim<string>
            {
                RoleId = role.Id,
                ClaimType = claim.Type,
                ClaimValue = claim.Value,
            };
    }
}

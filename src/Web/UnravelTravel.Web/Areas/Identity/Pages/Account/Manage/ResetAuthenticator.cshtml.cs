﻿namespace UnravelTravel.Web.Areas.Identity.Pages.Account.Manage
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using UnravelTravel.Data.Models;

#pragma warning disable SA1649 // File name should match first type name
    public class ResetAuthenticatorModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly UserManager<UnravelTravelUser> userManager;
        private readonly SignInManager<UnravelTravelUser> signInManager;
        private readonly ILogger<ResetAuthenticatorModel> logger;

        public ResetAuthenticatorModel(
            UserManager<UnravelTravelUser> userManager,
            SignInManager<UnravelTravelUser> signInManager,
            ILogger<ResetAuthenticatorModel> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            if (user == null)
            {
                return this.NotFound($"Unable to load user with ID '{this.userManager.GetUserId(this.User)}'.");
            }

            await this.userManager.SetTwoFactorEnabledAsync(user, false);
            await this.userManager.ResetAuthenticatorKeyAsync(user);
            this.logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

            await this.signInManager.RefreshSignInAsync(user);
            this.StatusMessage = "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

            return this.RedirectToPage("./EnableAuthenticator");
        }
    }
}

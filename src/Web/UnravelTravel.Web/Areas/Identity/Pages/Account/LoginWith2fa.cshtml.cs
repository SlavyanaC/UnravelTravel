﻿namespace UnravelTravel.Web.Areas.Identity.Pages.Account
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Web.Areas.Identity.Pages.Account.InputModels;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class LoginWith2FaModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<UnravelTravelUser> signInManager;
        private readonly ILogger<LoginWith2FaModel> logger;

        public LoginWith2FaModel(SignInManager<UnravelTravelUser> signInManager, ILogger<LoginWith2FaModel> logger)
        {
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [BindProperty]
        public LoginWith2faInputModel Input { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            this.ReturnUrl = returnUrl;
            this.RememberMe = rememberMe;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Page();
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new InvalidOperationException($"Unable to load two-factor authentication user.");
            }

            var authenticatorCode = this.Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await this.signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, this.Input.RememberMachine);

            if (result.Succeeded)
            {
                this.logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                return this.LocalRedirect(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                this.logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                this.logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                this.ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return this.Page();
            }
        }
    }
}

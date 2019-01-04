﻿namespace UnravelTravel.Web.Areas.Identity.Pages.Account
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.Extensions.Logging;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Areas.Identity.Pages.Account.InputModels;
    using UnravelTravel.Web.Common;
    using UnravelTravel.Web.Helpers;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class LoginModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<UnravelTravelUser> signInManager;
        private readonly ILogger<LoginModel> logger;
        private readonly IShoppingCartsService shoppingCartsService;

        public LoginModel(SignInManager<UnravelTravelUser> signInManager, ILogger<LoginModel> logger, IShoppingCartsService shoppingCartsService)
        {
            this.signInManager = signInManager;
            this.logger = logger;
            this.shoppingCartsService = shoppingCartsService;
        }

        [BindProperty]
        public LoginInputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(this.ErrorMessage))
            {
                this.ModelState.AddModelError(string.Empty, this.ErrorMessage);
            }

            returnUrl = returnUrl ?? this.Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await this.HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await this.signInManager.PasswordSignInAsync(this.Input.UserName, this.Input.Password, this.Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    this.logger.LogInformation("User logged in.");

                    var shoppingCartActivities = this.HttpContext.Session
                                                     .GetObjectFromJson<ShoppingCartActivityViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                                                 new List<ShoppingCartActivityViewModel>().ToArray();
                    if (shoppingCartActivities != null)
                    {
                        foreach (var activity in shoppingCartActivities)
                        {
                            await this.shoppingCartsService.AddActivityToShoppingCartAsync(activity.ActivityId, this.Input.UserName, activity.Quantity);
                        }

                        this.HttpContext.Session.Remove(WebConstants.ShoppingCartSessionKey);
                    }

                    return this.LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = this.Input.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    this.logger.LogWarning("User account locked out.");
                    return this.RedirectToPage("./Lockout");
                }
                else
                {
                    this.ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return this.Page();
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}

namespace UnravelTravel.Web.Areas.Identity.Pages.Account
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using UnravelTravel.Common;
    using UnravelTravel.Common.Extensions;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Models.ViewModels.ShoppingCart;
    using UnravelTravel.Services.Data.Contracts;
    using UnravelTravel.Web.Areas.Identity.Pages.Account.InputModels;
    using UnravelTravel.Web.Common;
    using UnravelTravel.Web.Helpers;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class ExternalLoginModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<UnravelTravelUser> signInManager;
        private readonly UserManager<UnravelTravelUser> userManager;
        private readonly ILogger<ExternalLoginModel> logger;
        private readonly IShoppingCartsService shoppingCartsService;

        public ExternalLoginModel(
            SignInManager<UnravelTravelUser> signInManager,
            UserManager<UnravelTravelUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IShoppingCartsService shoppingCartsService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.shoppingCartsService = shoppingCartsService;
        }

        [BindProperty]
        public ErrorLoginInputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGetAsync()
        {
            return this.RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = this.Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (remoteError != null)
            {
                this.ErrorMessage = $"Error from external provider: {remoteError}";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.ErrorMessage = "Error loading external login information.";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                this.logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                await this.StoreGuestShoppingCartIfAny(info.Principal.Identity.Name.RemoveWhiteSpaces());
                return this.LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                this.ReturnUrl = returnUrl;
                this.LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    this.Input = new ErrorLoginInputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    };
                }

                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.ErrorMessage = "Error loading external login information during confirmation.";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (this.ModelState.IsValid)
            {
                var nameWithoutWhiteSpaces = info.Principal.Identity.Name.RemoveWhiteSpaces();
                var user = new UnravelTravelUser
                {
                    UserName = nameWithoutWhiteSpaces,
                    Email = this.Input.Email,
                    ShoppingCart = new ShoppingCart(),
                };

                // For validating unique email
                IdentityResult result = null;
                IdentityError[] customErrors = null;
                try
                {
                    result = await this.userManager.CreateAsync(user);
                }
                catch (DbUpdateException ex)
                {
                    result = new IdentityResult();

                    if (ex.InnerException.Message.Contains("IX_AspNetUsers_Email"))
                    {
                        var exceptionMessage = $"User with email {user.Email} already exists. Please login and navigate to Account External Logins Add another service.";
                        customErrors = new[]
                        {
                            new IdentityError { Code = string.Empty, Description = exceptionMessage },
                        };
                    }
                }

                if (result.Succeeded)
                {
                    result = await this.userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await this.userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
                        await this.shoppingCartsService.AssignShoppingCartsUserId(user);

                        await this.signInManager.SignInAsync(user, isPersistent: false);
                        this.logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                        await this.StoreGuestShoppingCartIfAny(info.Principal.Identity.Name.RemoveWhiteSpaces());
                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors.Concat(customErrors))
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            this.LoginProvider = info.LoginProvider;
            this.ReturnUrl = returnUrl;
            return this.Page();
        }

        private async Task StoreGuestShoppingCartIfAny(string identityName)
        {
            var shoppingCartActivities = this.HttpContext.Session
                .GetObjectFromJson<ShoppingCartActivityViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                new List<ShoppingCartActivityViewModel>().ToArray();

            if (shoppingCartActivities != null)
            {
                foreach (var activity in shoppingCartActivities)
                {
                    await this.shoppingCartsService.AddActivityToShoppingCartAsync(activity.ActivityId, identityName, activity.Quantity);
                }

                this.HttpContext.Session.Remove(WebConstants.ShoppingCartSessionKey);
            }
        }
    }
}

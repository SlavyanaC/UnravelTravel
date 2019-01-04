namespace UnravelTravel.Web.Areas.Identity.Pages.Account
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
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
    public class RegisterModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<UnravelTravelUser> signInManager;
        private readonly UserManager<UnravelTravelUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IShoppingCartsService shoppingCartsService;

        public RegisterModel(
            UserManager<UnravelTravelUser> userManager,
            SignInManager<UnravelTravelUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IShoppingCartsService shoppingCartsService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.shoppingCartsService = shoppingCartsService;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public void OnGet(string returnUrl = null)
        {
            this.ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");
            if (this.ModelState.IsValid)
            {
                var user = new UnravelTravelUser
                {
                    UserName = this.Input.UserName,
                    FullName = this.Input.FullName,
                    Email = this.Input.Email,
                    ShoppingCart = new ShoppingCart(),
                };

                // For validating unique email
                IdentityResult result = null;
                IdentityError[] customErrors = null;
                try
                {
                    result = await this.userManager.CreateAsync(user, this.Input.Password);
                }
                catch (DbUpdateException ex)
                {
                    result = new IdentityResult();

                    if (ex.InnerException.Message.Contains("IX_AspNetUsers_Email"))
                    {
                        var exceptionMessage = $"User with email {user.Email} already exists.";
                        customErrors = new[]
                        {
                            new IdentityError { Code = string.Empty, Description = exceptionMessage },
                        };
                    }
                }

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    await this.userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
                    await this.shoppingCartsService.AssignShoppingCartsUserId(user);

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    var emailContent = callbackUrl.GetConfirmationEmailContent();
                    await this.emailSender.SendEmailAsync(
                        this.Input.Email,
                        "Confirm your email",
                        emailContent);

                    await this.signInManager.SignInAsync(user, isPersistent: false);

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

                foreach (var error in result.Errors.Concat(customErrors))
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}

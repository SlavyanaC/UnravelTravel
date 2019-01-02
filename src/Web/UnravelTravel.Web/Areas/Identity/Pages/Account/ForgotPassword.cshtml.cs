namespace UnravelTravel.Web.Areas.Identity.Pages.Account
{
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using UnravelTravel.Data.Models;
    using UnravelTravel.Web.Areas.Identity.Pages.Account.InputModels;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class ForgotPasswordModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IEmailSender emailSender;
        private IHostingEnvironment hostingEnvironment;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IHostingEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.hostingEnvironment = hostingEnvironment;
        }

        [BindProperty]
        public ForgotPasswordInputModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (this.ModelState.IsValid)
            {
                var user = await this.userManager.FindByEmailAsync(this.Input.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return this.RedirectToPage("./ForgotPasswordConfirmation");
                }

                if (!(await this.userManager.IsEmailConfirmedAsync(user)))
                {
                    // Send new confirmation email
                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    var content = this.GetConfirmationEmailContent(callbackUrl);

                    await this.emailSender.SendEmailAsync(
                        this.Input.Email,
                        "Confirm your email",
                        content);

                    return this.RedirectToPage("./ForgotPasswordConfirmation");
                }
                else
                {
                    // For more information on how to enable account confirmation and password reset please
                    // visit https://go.microsoft.com/fwlink/?LinkID=532713
                    var code = await this.userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = this.Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { code },
                        protocol: this.Request.Scheme);

                    var content = this.GetPasswordResetEmailContent(callbackUrl);
                    await this.emailSender.SendEmailAsync(
                        this.Input.Email,
                        "Reset Password",
                        content);

                    return this.RedirectToPage("./ForgotPasswordConfirmation");
                }
            }

            return this.Page();
        }

        // TODO: Find a better way to do this
        private string GetPasswordResetEmailContent(string callbackUrl)
        {
            var wwwrootPath = this.hostingEnvironment.WebRootPath;
            var passwordResetHtmlPath = $"{wwwrootPath}/password-reset.html";

            var doc = new HtmlDocument();
            doc.Load(passwordResetHtmlPath);

            var resetButton = @"<a href=""https://sendgrid.com"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Reset Password</a>";

            var newResetButton = $@"<a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Reset Password</a>";

            var resetLink =
                @"<p style=""margin: 0;""><a href=""https://sendgrid.com"" target=""_blank"">https://same-link-as-button.url/xxx-xxx-xxxx</a></p>";

            var newResetLink = $@"<p style=""margin: 0;""><a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"" target=""_blank"">{HtmlEncoder.Default.Encode(callbackUrl)}</a></p>";

            var content = doc.Text;
            content = content
                .Replace(resetButton, newResetButton)
                .Replace(resetLink, newResetLink);
            return content;
        }

        // TODO: Find a better way to do this
        private string GetConfirmationEmailContent(string callbackUrl)
        {
            var wwwrootPath = this.hostingEnvironment.WebRootPath;
            var emailConfirmationHtmlPath = $"{wwwrootPath}/email-confirmation.html";

            var doc = new HtmlDocument();
            doc.Load(emailConfirmationHtmlPath);

            var confirmationButton =
                @"<a href=""https://sendgrid.com"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Confirm email</a>";

            var newConfirmationButton =
                $@"<a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"" target=""_blank"" style=""display: inline-block; padding: 16px 36px; font-family: 'Source Sans Pro', Helvetica, Arial, sans-serif; font-size: 16px; color: #ffffff; text-decoration: none; border-radius: 6px;"">Confirm email</a>";

            var confirmationLink =
                @"<p style=""margin: 0;""><a href=""https://sendgrid.com"" target=""_blank"">https://same-link-as-button.url/xxx-xxx-xxxx</a></p>";

            var newConfirmationLink =
                $@"<p style=""margin: 0;""><a href=""{HtmlEncoder.Default.Encode(callbackUrl)}"" target=""_blank"">{HtmlEncoder.Default.Encode(callbackUrl)}</a></p>";

            var content = doc.Text;
            content = content
                .Replace(confirmationButton, newConfirmationButton)
                .Replace(confirmationLink, newConfirmationLink);
            return content;
        }
    }
}

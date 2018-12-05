﻿namespace UnravelTravel.Web.Areas.Identity.Pages
{
    using System.Diagnostics;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class ErrorModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public void OnGet()
        {
            this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
        }
    }
}

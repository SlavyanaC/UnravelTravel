namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Web.Controllers;

    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : BaseController
    {
        protected bool IsImageTypeValid(string fileType)
        {
            return fileType == ServicesDataConstants.JpgFormat || fileType == ServicesDataConstants.PngFormat || fileType == ServicesDataConstants.JpegFormat;
        }
    }
}

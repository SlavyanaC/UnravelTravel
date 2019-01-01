namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Services.Data.Common;
    using UnravelTravel.Web.Controllers;
    using UnravelTravel.Web.Filters;

    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    [AdminExceptionStoreExceptionFilter]
    public class AdministratorController : BaseController
    {
        protected bool IsImageTypeValid(string fileType)
        {
            return fileType == ServicesDataConstants.JpgFormat || fileType == ServicesDataConstants.PngFormat || fileType == ServicesDataConstants.JpegFormat;
        }
    }
}

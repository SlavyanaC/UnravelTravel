namespace UnravelTravel.Web.Areas.Administrator.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Web.Controllers;

    [Area("Administrator")]
    [Authorize(Roles = "Administrator")]
    public class AdministratorController : BaseController
    {
    }
}

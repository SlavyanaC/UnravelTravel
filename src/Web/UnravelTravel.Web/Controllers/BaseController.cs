namespace UnravelTravel.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using UnravelTravel.Web.Filters;

    [ModelStateValidationActionFilter]
    public class BaseController : Controller
    {
    }
}

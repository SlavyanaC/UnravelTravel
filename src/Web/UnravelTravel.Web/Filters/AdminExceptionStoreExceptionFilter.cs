namespace UnravelTravel.Web.Filters
{
    using System;

    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.DependencyInjection;
    using UnravelTravel.Data.Common.Repositories;
    using UnravelTravel.Data.Models;

    public class AdminExceptionStoreExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var adminExceptionsRepository =
                context.HttpContext.RequestServices.GetRequiredService<IRepository<AdminException>>();

            var adminException = new AdminException
            {
                AdminUserName = context.HttpContext.User.Identity.Name,
                ExceptionType = context.Exception.GetType().ToString(),
                ExceptionMessage = context.Exception.Message,
                OccurrenceDate = DateTime.UtcNow,
                CallingMethod = context.ActionDescriptor.DisplayName,
            };

            adminExceptionsRepository.Add(adminException);
            adminExceptionsRepository.SaveChangesAsync().GetAwaiter().GetResult();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using WorkService.MockApi.Models.Base;

namespace WorkService.MockApi.Attributes
{
    public class CustomExceptionAttribute : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            context.ExceptionHandled = true;
            context.Result = new CustomExceptionResult((int)status, context.Exception);
        }
    }
}

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using WorkService.MockApi.Models;
using System;
using WorkService.MockApi.Models.Base;

namespace WorkService.MockApi.Attributes
{
    public class ApiResultFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
        }
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ValidationFailedResult)
            {
                var objectResult = context.Result as ObjectResult;
                context.Result = objectResult;
            }
            else
            {
                var objectResult = context.Result as ObjectResult;
                context.Result = new OkObjectResult(new BaseResultModel(code: 200, result: objectResult.Value));
            }
        }
    }
}

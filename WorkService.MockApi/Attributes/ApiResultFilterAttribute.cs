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
            // ❗ 1. 非 ObjectResult 直接放行
            if (context.Result is not ObjectResult objectResult)
            {
                base.OnResultExecuting(context);
                return;
            }

            // ❗ 2. 防止 null Value
            var value = objectResult.Value;

            context.Result = new OkObjectResult(
                new BaseResultModel(
                    code: 200,
                    result: value
                )
            );
        }
    }
}

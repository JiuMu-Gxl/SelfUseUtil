using WorkService.MockApi.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace WorkService.MockApi.Attributes
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new ValidationFailedResult(context.ModelState);
            }
        }
        public class ValidationError
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Field { get; }
            public string Message { get; }
            public ValidationError(string field, string message)
            {
                Field = field != string.Empty ? field : null;
                Message = message;
            }
        }
    }
}

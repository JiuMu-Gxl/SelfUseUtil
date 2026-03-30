using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace WorkService.MockApi.Models.Base
{
    public class ValidationFailedResult : ObjectResult
    {

        public ValidationFailedResult(ModelStateDictionary modelState)
              : base(new ValidationFailedResultModel(modelState))
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity;
        }
    }
}

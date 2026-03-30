using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WorkService.MockApi.Models.Base
{
    public class CustomExceptionResult : ObjectResult
    {
        public CustomExceptionResult(int? code, Exception exception)
                : base(new CustomExceptionResultModel(code, exception))
        {
            StatusCode = code;
        }
    }
}

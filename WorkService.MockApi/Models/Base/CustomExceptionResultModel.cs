using Microsoft.AspNetCore.Mvc;
using System;

namespace WorkService.MockApi.Models.Base
{
    public class CustomExceptionResultModel : BaseResultModel
    {
        public CustomExceptionResultModel(int? code, Exception exception)
        {
            Code = code;
            Message = exception.InnerException != null ?
                exception.InnerException.Message :
                exception.Message;
            Result = null;
        }
    }
}

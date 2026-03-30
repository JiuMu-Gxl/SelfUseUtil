namespace WorkService.MockApi.Models.Base
{
    public class BaseResultModel
    {
        public BaseResultModel(int? code = null, string message = null,
            object result = null)
        {
            Code = code;
            Result = result;
            Message = message;
            Success = code == 200;
        }
        public int? Code { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }
    }
}

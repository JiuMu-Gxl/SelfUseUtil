using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RedisDemo.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public async Task<string> Test(string str) {
            Console.WriteLine($"这是参数：{str}");
            return "这是回参";
        }
    }
}

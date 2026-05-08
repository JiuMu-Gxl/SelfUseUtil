using Microsoft.AspNetCore.Mvc;
using WorkService.MockApi.Dtos.Iot;
using WorkService.MockApi.Services;

namespace WorkService.MockApi.Controllers.Iot
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public MqttUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateUser(string username, string password)
        {
            await _userService.CreateUserAsync(username, password);
            return Ok("创建成功");
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(string username, string newPassword)
        {
            await _userService.ChangePasswordAsync(username, newPassword);
            return Ok("修改成功");
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser(string username)
        {
            await _userService.DeleteUserAsync(username);
            return Ok("删除成功");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            await _userService.LoginAsync(req.Username, req.Password);
            return Ok("登录成功");
        }
    }
}

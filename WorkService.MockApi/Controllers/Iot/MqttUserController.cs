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
        public async Task CreateUser(string username, string password)
        {
            await _userService.CreateUserAsync(username, password);
        }

        [HttpPost("change-password")]
        public async Task ChangePassword(string username, string newPassword)
        {
            await _userService.ChangePasswordAsync(username, newPassword);
        }

        [HttpDelete("delete")]
        public async Task DeleteUser(string username)
        {
            await _userService.DeleteUserAsync(username);
        }

        [HttpPost("login")]
        public async Task Login([FromBody] LoginRequest req)
        {
            await _userService.LoginAsync(req.Username, req.Password);
        }
    }
}

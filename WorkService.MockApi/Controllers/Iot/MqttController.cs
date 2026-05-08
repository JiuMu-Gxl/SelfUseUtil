using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkService.MockApi.Dtos.Iot;
using WorkService.MockApi.Services;

namespace WorkService.MockApi.Controllers.Iot
{
    [Route("api/[controller]")]
    [ApiController]
    public class MqttController : ControllerBase
    {
        private readonly IMqttService _mqttService;

        public MqttController(IMqttService mqttService)
        {
            _mqttService = mqttService;
        }

        [HttpPost("test-connect")]
        public async Task<IActionResult> TestConnect([FromBody] LoginRequest req)
        {
            await _mqttService.TestConnectAsync(req.Username, req.Password);
            return Ok("连接成功");
        }


        [HttpPost("publish")]
        public async Task<IActionResult> Publish([FromBody] PublishRequest req)
        {
            await _mqttService.PublishAsync(req.Topic, req.Payload);
            return Ok("发布成功");
        }

        [HttpGet("subscribe/{Topic}")]
        public async Task<IActionResult> Subscribe(string Topic)
        {
            await _mqttService.SubscribeAsync(Topic);
            return Ok("订阅成功");
        }

        [HttpGet("disconnect/{Username}")]
        public async Task<IActionResult> Disconnect(string Username)
        {
            await _mqttService.DisconnectAsync(Username);
            return Ok("已断开连接");
        }
    }
}

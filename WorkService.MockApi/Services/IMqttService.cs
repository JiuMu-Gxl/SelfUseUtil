using Microsoft.Extensions.Options;
using MQTTnet;
using System.Collections.Concurrent;
using System.Text;
using WorkService.MockApi.Dtos.Iot;
using WorkService.MockApi.Helpers;
using WorkService.MockApi.Repositorys;

namespace WorkService.MockApi.Services
{
    public interface IMqttService
    {
        Task TestConnectAsync(string username, string password);

        Task PublishAsync(string topic, string payload);

        Task SubscribeAsync(string topic);

        Task DisconnectAsync(string username);
    }

    public class MqttService : IMqttService
    {
        private readonly IUserRepository _userRepo;
        private readonly MqttOptions _options;

        // ✔ 连接池（关键）
        private static readonly ConcurrentDictionary<string, IMqttClient> _clients = new();

        public MqttService(
            IUserRepository userRepo,
            IOptions<MqttOptions> options)
        {
            _userRepo = userRepo;
            _options = options.Value;
        }

        #region TestConnect
        public async Task TestConnectAsync(string username, string password)
        {
            var user = await _userRepo.GetByUsernameAsync(username);
            if (user == null)
                throw new Exception("用户不存在");

            var hash = PasswordHelper.Md5WithSalt(password, user.Salt);
            if (hash != user.PasswordHash)
                throw new Exception("用户名或密码错误");

            var factory = new MqttClientFactory();
            var client = factory.CreateMqttClient();

            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_options.Host, _options.Port)
                .WithClientId($"{_options.ClientIdPrefix}-{username}")
                .WithCredentials(username, password)
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(_options.KeepAlive))
                .Build();

            var result = await client.ConnectAsync(options);

            if (result.ResultCode != MqttClientConnectResultCode.Success)
                throw new Exception("连接失败");

            // ✔ 保存连接（关键）
            _clients[username] = client;
        }
        #endregion

        #region Publish
        public async Task PublishAsync(string topic, string payload)
        {
            var client = GetClient();

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(Encoding.UTF8.GetBytes(payload))
                .Build();

            await client.PublishAsync(message);
        }
        #endregion

        #region Subscribe
        public async Task SubscribeAsync(string topic)
        {
            var client = GetClient();

            await client.SubscribeAsync(topic);
        }
        #endregion

        #region Disconnect
        public async Task DisconnectAsync(string username)
        {
            if (_clients.TryRemove(username, out var client))
            {
                await client.DisconnectAsync();
                client.Dispose();
            }
        }
        #endregion

        #region Helper
        private IMqttClient GetClient()
        {
            if (_clients.IsEmpty)
                throw new Exception("MQTT未连接");

            return _clients.Values.First();
        }
        #endregion
    }
}

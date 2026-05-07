namespace WorkService.MockApi.Dtos.Iot
{
    public class MqttOptions
    {
        public string Host { get; set; } = null!;
        public int Port { get; set; }
        public string ClientIdPrefix { get; set; } = null!;
        public int KeepAlive { get; set; }
    }
}

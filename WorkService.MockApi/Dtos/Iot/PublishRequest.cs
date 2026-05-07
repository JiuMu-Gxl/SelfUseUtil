namespace WorkService.MockApi.Dtos.Iot
{
    public class PublishRequest
    {
        public string Topic { get; set; } = null!;
        public string Payload { get; set; } = null!;
    }
}

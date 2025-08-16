namespace MessageProducerService.Models
{
    public class MessageBrokerSettings
    {
        public string HostName { get; set; }
        public int Port { get; set; } = 5672;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; } = "/";
        public ushort PrefetchCount { get; set; } = 10;
    }
}

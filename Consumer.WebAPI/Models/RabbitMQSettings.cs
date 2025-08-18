namespace Consumer.WebAPI.Models
{
    public class RabbitMQSettings
    {
        public string HostName { get; set; } = default!;
        public int Port { get; set; }
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string VirtualHost { get; set; } = "/";
        public ushort PrefetchCount { get; set; } = 10;
    }
}

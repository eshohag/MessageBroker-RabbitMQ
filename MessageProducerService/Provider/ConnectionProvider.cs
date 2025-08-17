using MessageProducerService.Models;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MessageProducerService.Provider
{
    public interface IConnectionProvider
    {
        IConnection Connection { get; }
    }

    public class ConnectionProvider : BackgroundService, IConnectionProvider
    {
        private readonly MessageBrokerSettings _settings;
        private readonly ILogger<ConnectionProvider> _logger;

        public IConnection Connection { get; private set; }

        public ConnectionProvider(IOptions<MessageBrokerSettings> options, ILogger<ConnectionProvider> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _settings.HostName,
                    Port = _settings.Port,
                    UserName = _settings.UserName,
                    Password = _settings.Password,
                    VirtualHost = _settings.VirtualHost,
                    AutomaticRecoveryEnabled = true
                };
                _logger.LogInformation("Connecting to RabbitMQ at {Host}:{Port}", _settings.HostName, _settings.Port);
                Connection = await factory.CreateConnectionAsync();
                _logger.LogInformation("RabbitMQ connection established successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to connect to RabbitMQ.");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Connection?.Dispose();
            await base.StopAsync(cancellationToken);
        }
    }
}

using MessageProducerService.Provider;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MessageProducerService.Services
{
    // IMessageProducerService.cs
    public interface IMessageProducerService
    {
         Task PublishAsync<T>(string exchange, string routingKey, T message, string? ensureQueue = null);
    }

    public class MessageProducerService : IMessageProducerService
    {
        private readonly IConnectionProvider _provider;
        private readonly ILogger<MessageProducerService> _logger;

        public MessageProducerService(
            IConnectionProvider provider,
            ILogger<MessageProducerService> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        public async Task PublishAsync<T>(string exchange, string routingKey, T message, string? ensureQueue = null)
        {
            var conn = _provider.Connection;
            if (conn is null || !conn.IsOpen)
                throw new InvalidOperationException("RabbitMQ connection is not open. Ensure the connection provider is initialized before publishing.");

            using var channel = conn.CreateModel();

            // Declare topology (idempotent)
            channel.ExchangeDeclare(exchange: exchange, type: ExchangeType.Direct, durable: true);

            // Optional: ensure a queue exists and is bound (great for local/testing)
            if (!string.IsNullOrWhiteSpace(ensureQueue))
            {
                channel.QueueDeclare(queue: ensureQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
                channel.QueueBind(queue: ensureQueue, exchange: exchange, routingKey: routingKey);
            }

            // Detect unrouted messages
            channel.BasicReturn += (_, ea) =>
            {
                _logger.LogError(
                    "Message was returned by broker. Code={Code}, Text={Text}, Exchange={Exchange}, RoutingKey={RoutingKey}",
                    ea.ReplyCode, ea.ReplyText, ea.Exchange, ea.RoutingKey);
            };

            // Publisher confirms (so PublishAsync has real meaning)
            channel.ConfirmSelect();

            var body = JsonSerializer.SerializeToUtf8Bytes(message);
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "application/json";

            // mandatory:true => broker returns message if unrouted
            channel.BasicPublish(
                exchange: exchange,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: props,
                body: body);

            // Wait for confirm (throws on NACK / timeout)
            channel.WaitForConfirmsOrDie(TimeSpan.FromSeconds(5));

            _logger.LogInformation("Published {Bytes} bytes to {Exchange}:{RoutingKey}", body.Length, exchange, routingKey);

            return Task.CompletedTask;
        }
    }
}

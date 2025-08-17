using MessageProducerService.Provider;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace MessageProducerService.Services
{
    public interface IProducerService
    {
        Task PublishAsync<T>(string exchangeName, string type, string routingKey, string queueName, T message);
    }

    public class ProducerService : IProducerService
    {
        private readonly ILogger<ProducerService> _logger;
        private readonly IConnectionProvider _connectionProvider;

        public ProducerService(ILogger<ProducerService> logger, IConnectionProvider connectionProvider)
        {
            _logger = logger;
            _connectionProvider = connectionProvider;
        }

        public async Task PublishAsync<T>(string exchangeName, string type, string routingKey, string queueName, T message)
        {
            IConnection connection = null;
            IChannel channel = null;
            try
            {
                connection = _connectionProvider.Connection;
                if (connection is null || !connection.IsOpen)
                    throw new InvalidOperationException("RabbitMQ connection is not open. Ensure the connection provider is initialized before publishing.");
                channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(exchangeName, type, durable: true, autoDelete: false, arguments: null, noWait: false, cancellationToken: default);
                await channel.QueueDeclareAsync(queue: queueName,
                    durable: true, // save to disk so the queue isn’t lost on broker restart
                    exclusive: false, // can be used by other connections
                    autoDelete: false, // don’t delete when the last consumer disconnects
                    arguments: null);
                await channel.QueueBindAsync(queueName, exchangeName, routingKey, arguments: null, noWait: false, cancellationToken: default);

                var bodyBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

                // Publish the message
                await channel.BasicPublishAsync(
                    exchange: exchangeName,
                    routingKey: routingKey,
                    mandatory: true,
                    basicProperties: new BasicProperties() { Persistent = true, ContentType = "application/json" },
                    body: bodyBytes);
                //await channel.CloseAsync();
                //await connection.CloseAsync();
                _logger.LogInformation("Published {Bytes} bytes to {Exchange}:{RoutingKey}", bodyBytes.Length, exchangeName, routingKey);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
               
            }
        }
    }
}

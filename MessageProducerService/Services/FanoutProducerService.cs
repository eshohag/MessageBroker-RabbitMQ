using MessageProducerService.Provider;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace MessageProducerService.Services
{
    public interface IFanoutProducerService
    {
        Task PublishAsync<T>(string exchangeName, string type, string routingKey, string queueName, T message);
    }

    public class FanoutProducerService : IFanoutProducerService
    {
        private readonly ILogger<DirectProducerService> _logger;
        private readonly IConnectionProvider _connectionProvider;

        public FanoutProducerService(ILogger<DirectProducerService> logger, IConnectionProvider connectionProvider)
        {
            _logger = logger;
            _connectionProvider = connectionProvider;
        }

        public async Task PublishAsync<T>(string exchangeName, string type, string routingKey, string queueName, T message)
        {
            try
            {
               var connection = _connectionProvider.Connection;
                if (connection is null || !connection.IsOpen)
                    throw new InvalidOperationException("RabbitMQ connection is not open. Ensure the connection provider is initialized before publishing.");
               var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(exchangeName, type, 
                    durable: true, // save to disk so the queue isn’t lost on broker restart
                    autoDelete: false, 
                    arguments: null, 
                    noWait: false,
                    cancellationToken: default);

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

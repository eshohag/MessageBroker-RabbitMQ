using Email.Consumer.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Email.Consumer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string exchangeName = "ExchangeNameTopic";
            string queueName = "EmailTopicQueue";
            string routingKey = "order.*";

            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest",
                VirtualHost = "/",
                AutomaticRecoveryEnabled = true
            };
            using IConnection connection = await factory.CreateConnectionAsync();
            using IChannel channel = await connection.CreateChannelAsync();

            // Consumer setup for fanout
            await channel.ExchangeDeclareAsync(exchange: exchangeName, durable: true, autoDelete: false, type: ExchangeType.Topic);
            await channel.QueueDeclareAsync(queue: queueName,
                                  durable: false,  //If you want durable queues (instead of temporary), give each consumer a fixed queue name and mark it durable: true. Temporary queues are deleted once the consumer disconnects.
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);
            await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey, arguments: null, noWait: false, cancellationToken: default);

            Console.WriteLine(" [*] Email Waiting for messages.");

            // Define a consumer and start listening
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                var orderPlaced = JsonSerializer.Deserialize<Message>(message);

                Console.WriteLine($"Received: Order Name - {orderPlaced.Name}, Address - {orderPlaced.Address}");
            };
            await channel.BasicConsumeAsync(queueName, autoAck: true, consumer);
            Console.WriteLine("Waiting for messages...");
            Console.ReadLine();
        }
    }
}

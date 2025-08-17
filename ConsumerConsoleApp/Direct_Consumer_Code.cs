using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Runtime;
using System.Text;
using System.Text.Json;

namespace ConsumerConsoleApp
{
    public class Direct_Consumer_Code
    {
        static async Task Direct_Main(string[] args)
        {
            string exchangeName = "ExchangeNameDirect";
            string queueName = "DirectQueue";
            string routingKey = "Abcd@1234";

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
            await channel.ExchangeDeclareAsync(exchange: exchangeName, durable: true, autoDelete: false, type: ExchangeType.Direct);
            await channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null, noWait: false, cancellationToken: default);
            await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: routingKey, arguments: null, noWait: false, cancellationToken: default);

            Console.WriteLine(" [*] Waiting for messages.");

            // Define a consumer and start listening
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, eventArgs) =>
            {
                byte[] body = eventArgs.Body.ToArray();
                string message = Encoding.UTF8.GetString(body);
                var orderPlaced = JsonSerializer.Deserialize<Message>(message);

                Console.WriteLine($"Received: Order Name - {orderPlaced.Name}, Address - {orderPlaced.Address}");

                // Acknowledge the message
                await ((AsyncEventingBasicConsumer)sender)
                    .Channel.BasicAckAsync(eventArgs.DeliveryTag, multiple: false);
            };
            await channel.BasicConsumeAsync(queueName, autoAck: false, consumer);
            Console.WriteLine("Waiting for messages...");
            Console.ReadLine();
        }
    }
}

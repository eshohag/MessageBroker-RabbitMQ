using Consumer.WebAPI.Models;
using MassTransit;

namespace Consumer.WebAPI.Services
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            var message = $"[x] Received Order {context.Message.OrderId} for {context.Message.CustomerName}, Price {context.Message.TotalAmount}";
            Console.WriteLine(message);

            // Add business logic here

            return Task.CompletedTask;
        }
    }
}

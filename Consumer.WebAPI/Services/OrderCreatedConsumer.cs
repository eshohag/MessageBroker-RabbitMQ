using Consumer.WebAPI.Models;
using MassTransit;

namespace Consumer.WebAPI.Services
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }
        public Task Consume(ConsumeContext<OrderCreated> context)
        {
            _logger.LogInformation($"[x] Received order: {context.Message.OrderId} " +
                                  $"From: {context.Message.CustomerName} " +
                                  $"Amount: {context.Message.TotalAmount}");
            Console.WriteLine($"[x] Received Order {context.Message.OrderId} for {context.Message.CustomerName}, Price {context.Message.TotalAmount}");

            // Add business logic here

            return Task.CompletedTask;
        }
    }
}

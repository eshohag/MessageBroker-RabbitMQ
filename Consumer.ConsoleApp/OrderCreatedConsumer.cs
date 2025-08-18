using Consumer.ConsoleApp.Models;
using MassTransit;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer.ConsoleApp
{
    public class OrderCreatedConsumer : IConsumer<OrderCreated>
    {
        private readonly ILogger<OrderCreatedConsumer> _logger;

        public OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {
            _logger.LogInformation($"Received order: {context.Message.OrderId} " +
                                  $"From: {context.Message.CustomerName} " +
                                  $"Amount: {context.Message.TotalAmount}");

            // Add business logic here
        }
    }
}

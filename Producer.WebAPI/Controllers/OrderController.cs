using MassTransit;
using MassTransit.Serialization;
using MassTransit.Transports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Producer.WebAPI.Models;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace Producer.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IBus _bus;
        public OrderController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreated order)
        {
            /*  ********Final Notes********
                Always use URI scheme: queue: or exchange:.
                For fanout exchanges → routing key is ignored.
                For direct exchanges → routing key must match.
                For topic exchanges → routing key is a pattern.

                Send to Exchange → use exchange:exchange-name + SetRoutingKey.
                Send to Queue → use queue:queue-name (no routing key needed).
                Don’t mix exchange: + queue: in the same send.
             */

            // 1. For direct exchanges → routing key must match, Don’t mix exchange + queue in the same send/Published.
            //var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:order-exchange"));
            //await endpoint.Send<OrderCreated>(order, context =>
            //{
            //    // Routing key applies only to direct/topic exchanges
            //    context.SetRoutingKey("direct-order-my-routing-key");  //For fanout exchange-> routing key is ignored.
            //    //context.DestinationAddress = new Uri("queue:order-queue"); //For fanout exchange-> queue name is ignored, For topic exchnage-> queue name is ignored.
            //    context.Durable = true; // Ensure the message is persisted in RabbitMQ
            //});

            //Direct Exchange
            //Messages are routed to queues whose binding key exactly matches the routing key.
            var orderMq = new OrderCreated() { OrderId = Guid.NewGuid(), CustomerName = order.CustomerName, TotalAmount = order.TotalAmount };

            var uri = new Uri($"exchange:my.orders.exchange?type=topic");

            var endpoint = await _bus.GetSendEndpoint(uri);

            await endpoint.Send(orderMq, ctx =>
            {
                ctx.SetRoutingKey("orders.submitted");
            });

            return Ok(new { Message = "Order sent successfully", order.OrderId });
        }
    }
}
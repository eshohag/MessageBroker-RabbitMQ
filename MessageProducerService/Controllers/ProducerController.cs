using MessageProducerService.Models;
using MessageProducerService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Threading.Tasks;

namespace MessageProducerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducerController : ControllerBase
    {
        private readonly IProducerService _producerService;

        public ProducerController(IProducerService producerService)
        {
            _producerService = producerService;
        }

        [HttpPost("message/sent")]
        public async Task<IActionResult> MessageSent([FromBody] Message message)
        {
            await _producerService.PublishAsync(exchangeName: "TestExchangeName", type: ExchangeType.Direct, routingKey: "messageXYZ", queueName: "EnsureQueue1XYZ", message);
            return Ok("Message has been sent successfully");
        }
    }
}

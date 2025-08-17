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
        private readonly IDirectProducerService _directProducerService;
        private readonly IFanoutProducerService _fanoutProducerService;
        private readonly ITopicProducerService _topicProducerService;

        public ProducerController(IDirectProducerService directProducerService, IFanoutProducerService fanoutProducerService, ITopicProducerService topicProducerService)
        {
            _directProducerService = directProducerService;
            _fanoutProducerService = fanoutProducerService;
            _topicProducerService = topicProducerService;
        }

        [HttpPost("message/direct")]
        public async Task<IActionResult> MessageSentDirect([FromBody] Message message)
        {
            await _directProducerService.PublishAsync(exchangeName: "ExchangeNameDirect", type: ExchangeType.Direct, routingKey: "Abcd@1234", queueName: "DirectQueue", message);
            return Ok("Message has been sent successfully");
        }
        [HttpPost("message/fanout")]
        public async Task<IActionResult> MessageSentFanout([FromBody] Message message)
        {
            await _fanoutProducerService.PublishAsync(exchangeName: "ExchangeNameFanout", type: ExchangeType.Fanout, routingKey: "", queueName: "", message);
            return Ok("Message has been sent successfully");
        }

        [HttpPost("message/topic")]
        public async Task<IActionResult> MessageSentTopic([FromBody] Message message)
        {
            await _topicProducerService.PublishAsync(exchangeName: "ExchangeNameTopic", type: ExchangeType.Topic, routingKey: "order.created.us", queueName: "", message);
            return Ok("Message has been sent successfully");
        }
    }
}

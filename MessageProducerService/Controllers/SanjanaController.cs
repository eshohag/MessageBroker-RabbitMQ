using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessageProducerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanjanaController : ControllerBase
    {
        private readonly ILogger<SanjanaController> _logger;
        public SanjanaController(ILogger<SanjanaController> logger)
        {
            _logger = logger;
        }

        [HttpPost("hello")]
        public IActionResult GetHello()
        {
            _logger.LogInformation("Hello endpoint was called.");
            return Ok("Hello from SanjanaController!");
        }

    }
}

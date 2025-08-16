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
            //Sadia Works Here
            //Sanjana Works Here
            //sanjana2 works here
            //Shohag2 works her
            //Shohag3 works her
            //Ruby wants to learn
            //Teacher develop cheathing tracker
            //Shohag commened this line 
            //Student solve the problem 
            //Sanjana commented this line
            //try reset this comments
            _logger.LogInformation("Hello endpoint was called.");
            return Ok("Hello from SanjanaController!");
        }

    }
}

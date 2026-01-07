using Microsoft.AspNetCore.Mvc;

namespace SearchSync.WebApiKafka.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("~/health")]
        public IActionResult GetHealth()
        {
            var health = new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow.ToString("O"),
                service = "Kafka Listener"
            };

            return Ok(health);
        }
    }
}

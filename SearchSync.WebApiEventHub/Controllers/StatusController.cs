using Microsoft.AspNetCore.Mvc;

namespace SearchSync.WebApiEventHub.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Health check endpoint for the API.
        /// </summary>
        /// <returns>200 OK with API status</returns>
        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Health()
        {
            _logger.LogDebug("Health check requested");
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                service = "EventHub Listener"
            });
        }

        /// <summary>
        /// Information endpoint showing service details.
        /// </summary>
        /// <returns>200 OK with service information</returns>
        [HttpGet("info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Info()
        {
            _logger.LogDebug("Info requested");
            return Ok(new
            {
                service = "SearchSync EventHub Listener",
                version = "1.0.0",
                description = "Listens to Azure EventHub for OrderCloud events and syncs them to Sitecore Search",
                timestamp = DateTime.UtcNow
            });
        }
    }
}

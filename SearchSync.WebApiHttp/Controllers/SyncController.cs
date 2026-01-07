using Microsoft.AspNetCore.Mvc;
using SearchSync.Common.EventContext.Adapters;
using SearchSync.Common.Services;

namespace SearchSync.WebApiHttp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SyncController : ControllerBase
    {
        private readonly ILogger<SyncController> _logger;
        private readonly IEventParser _eventParser;
        private readonly ISearchIngestionService _searchIngestionService;

        public SyncController(
            ILogger<SyncController> logger,
            IEventParser eventParser,
            ISearchIngestionService searchIngestionService)
        {
            _logger = logger;
            _eventParser = eventParser;
            _searchIngestionService = searchIngestionService;
        }

        /// <summary>
        /// Processes a SearchSync event by parsing it and ingesting it into the search service.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/sync/process
        ///     {
        ///       "Headers": {
        ///         "type": "sitecore.ordercloud.messages.product.updated"
        ///       },
        ///       "Payload": {
        ///         "id": "PROD-001",
        ///         "name": "Product Name",
        ///         "description": "Product description",
        ///         "active": true,
        ///         "archived": false
        ///       }
        ///     }
        /// </remarks>
        /// <returns>200 OK if the event is successfully processed</returns>
        /// <response code="200">Event successfully processed</response>
        /// <response code="400">Invalid event format or missing required fields</response>
        /// <response code="500">An error occurred while processing the event</response>
        [HttpPost("process")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessEvent()
        {
            _logger.LogDebug("HTTP POST request received at /api/sync/process");

            try
            {
                var requestBodyString = await new StreamReader(Request.Body).ReadToEndAsync();

                if (string.IsNullOrEmpty(requestBodyString))
                {
                    _logger.LogWarning("Request body is empty");
                    return BadRequest(new { error = "Request body is required" });
                }

                var context = new HttpEventContext(requestBodyString);
                var (eventType, payload) = _eventParser.Parse(context);

                _logger.LogInformation("Processing event of type: {EventType}", eventType);
                await _searchIngestionService.ProcessEvent(eventType, payload);

                _logger.LogInformation("Successfully processed event of type: {EventType}", eventType);
                return Ok(new { message = "Event processed successfully", eventType = eventType });
            }
            catch (Exception ex)
            {
                if (ex is ILoggableException loggable)
                {
                    using (_logger.BeginScope(loggable.GetCustomDimensions()))
                    {
                        _logger.LogError(ex, loggable.GetLogMessage());
                    }
                    return BadRequest(new { error = loggable.GetLogMessage() });
                }
                else
                {
                    _logger.LogError(ex, "An unexpected error occurred while processing the event: {Message}", ex.Message);
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { error = "An error occurred while processing the event", details = ex.Message });
                }
            }
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
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}

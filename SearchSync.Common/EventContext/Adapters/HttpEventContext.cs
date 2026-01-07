using Newtonsoft.Json;
using SearchSync.Common.Models;

namespace SearchSync.Common.EventContext.Adapters
{
    public class HttpEventContext : IEventContext
    {
        private readonly HttpSearchIntegrationEvent? _integrationEvent;

        public HttpEventContext(string requestBodyString)
        {
            _integrationEvent = JsonConvert.DeserializeObject<HttpSearchIntegrationEvent>(requestBodyString);
        }

        public string Body => _integrationEvent?.Payload.ToString() ?? string.Empty;

        public string? GetProperty(string key)
        {
            if (_integrationEvent?.Headers == null)
            {
                return null;
            }
            return _integrationEvent.Headers.TryGetValue(key, out var value) ? value : null;
        }
    }
}

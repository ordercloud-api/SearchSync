using Azure.Messaging.EventHubs;
using System.Text;

namespace SearchSync.Common.EventContext.Adapters
{
    public class EventHubEventContext : IEventContext
    {
        private readonly EventData _event;

        public EventHubEventContext(EventData eventData)
        {
            _event = eventData;
        }

        public string Body => Encoding.UTF8.GetString(_event.Body.ToArray());

        public string? GetProperty(string key)
            => _event.Properties.TryGetValue(key, out var value) ? value?.ToString() : null;
    }
}

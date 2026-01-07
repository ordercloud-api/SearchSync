using Newtonsoft.Json.Linq;
using System.Text;

namespace SearchSync.Common.EventContext.Adapters
{
    /// <summary>
    /// Adapter to convert Kafka message format to IEventContext interface.
    /// Kafka messages can have the event type in headers, the message key, or the message body.
    /// </summary>
    public class KafkaEventContext : IEventContext
    {
        private readonly string _key;
        private readonly string _body;
        private readonly Dictionary<string, byte[]>? _headers;
        private JObject? _bodyJson;

        /// <summary>
        /// Constructor accepting message key, body, and optional headers as a dictionary.
        /// </summary>
        public KafkaEventContext(string? key, string body, Dictionary<string, byte[]>? headers = null)
        {
            _key = key ?? "";
            _body = body;
            _headers = headers;
            
            // Try to parse body as JSON for property extraction
            try
            {
                _bodyJson = JObject.Parse(body);
            }
            catch
            {
                // If it's not valid JSON, we'll just use string-based property access
                _bodyJson = null;
            }
        }

        public string Body => _body;

        /// <summary>
        /// Gets a property from the Kafka message.
        /// Checks in order: message headers, message key, then message body JSON.
        /// </summary>
        public string? GetProperty(string key)
        {
            // First, try to get from message headers (highest priority)
            if (_headers != null && _headers.Count > 0)
            {
                // For "type" field, also check CloudEvents convention header "ce_type"
                var keysToCheck = key.Equals("type", StringComparison.OrdinalIgnoreCase) 
                    ? new[] { key, "ce_type" } 
                    : new[] { key };

                foreach (var checkKey in keysToCheck)
                {
                    // Try to find header with case-insensitive key match
                    var headerKey = _headers.Keys.FirstOrDefault(k => k.Equals(checkKey, StringComparison.OrdinalIgnoreCase));
                    if (headerKey != null && _headers.TryGetValue(headerKey, out var headerValue))
                    {
                        return Encoding.UTF8.GetString(headerValue);
                    }
                }
            }

            // Then try to get from parsed JSON body
            if (_bodyJson != null)
            {
                // Try exact case match first
                if (_bodyJson.TryGetValue(key, out var value))
                {
                    return value?.ToString();
                }

                // For "type" field, check common alternate names (case-insensitive)
                if (key.Equals("type", StringComparison.OrdinalIgnoreCase))
                {
                    var typeFieldNames = new[] { "type", "eventType", "EventType", "event_type", "Type", "TYPE", "MessageType", "messageType" };
                    
                    foreach (var fieldName in typeFieldNames)
                    {
                        // Try case-insensitive match
                        var token = _bodyJson.Children<JProperty>()
                            .FirstOrDefault(p => p.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
                        
                        if (token?.Value != null)
                        {
                            return token.Value.ToString();
                        }
                    }

                    // Log the body structure if type not found
                    var bodyKeys = string.Join(", ", _bodyJson.Properties().Select(p => p.Name));
                    System.Diagnostics.Debug.WriteLine($"[KafkaEventContext] Looking for 'type' in body. Available properties: {bodyKeys}");
                }
            }

            return null;
        }
    }
}

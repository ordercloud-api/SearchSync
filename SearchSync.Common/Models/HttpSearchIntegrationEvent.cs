namespace SearchSync.Common.Models
{
    public class HttpSearchIntegrationEvent
    {
        public Dictionary<string, string?> Headers { get; set; }
        public object Payload { get; set; }
    }
}

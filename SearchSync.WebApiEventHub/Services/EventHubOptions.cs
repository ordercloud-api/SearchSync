namespace SearchSync.WebApiEventHub.Services
{
    /// <summary>
    /// Configuration options for EventHub listener.
    /// </summary>
    public class EventHubOptions
    {
        public string? ConnectionString { get; set; }
        public string? EventHubName { get; set; }
        public string? ConsumerGroup { get; set; }
        public string? StorageConnectionString { get; set; }
        public string? StorageContainerName { get; set; }
    }
}

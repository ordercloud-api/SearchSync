namespace SearchSync.WebApiKafka.Services
{
    /// <summary>
    /// Configuration options for Kafka listener.
    /// </summary>
    public class KafkaOptions
    {
        public string? BootstrapServers { get; set; }
        public string? Topic { get; set; }
        public string? GroupId { get; set; }
        public int? SessionTimeoutMs { get; set; }
        public int? FetchMinBytes { get; set; }
        public int? FetchWaitMaxMs { get; set; }
        
        // Security/Authentication settings (for Azure Event Hub or secured Kafka)
        public string? SecurityProtocol { get; set; }
        public string? SaslMechanism { get; set; }
        public string? SaslUsername { get; set; }
        public string? SaslPassword { get; set; }
    }
}

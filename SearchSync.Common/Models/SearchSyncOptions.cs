namespace SearchSync.Common.Models
{
    /// <summary>
    /// Provides a unified set of configuration options for the SearchSync service.
    /// </summary>
    public class SearchSyncOptions
    {
        // Client Connection Settings
        public string IngestionEndpointUrl { get; set; }
        public string IngestionApiKey { get; set; }
        public string DomainId { get; set; }
        public string SourceId { get; set; }

        // Entity Name Settings
        public string EntityProduct { get; set; }
        public string EntityCategory { get; set; }
        public string EntityAdminUser { get; set; }
        public string EntityBuyer { get; set; }
        public string EntityBuyerUserGroup { get; set; }
        public string EntityBuyerUser { get; set; }
        public string EntityInventoryRecord { get; set; }
        public string EntitySupplier { get; set; }
        public string EntitySupplierUser { get; set; }
    }
}

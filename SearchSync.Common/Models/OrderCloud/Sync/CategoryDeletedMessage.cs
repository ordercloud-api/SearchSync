namespace SearchSync.Common.Models.OrderCloud.Sync
{
    public class CategoryDeletedMessage
    {
        public UserContext UserContext { get; set; }
        public string CatalogID { get; set; }
        public string ID { get; set; }
    }
}

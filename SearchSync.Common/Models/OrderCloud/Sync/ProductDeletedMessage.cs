using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Models.OrderCloud.ProductSync
{
    public class ProductDeletedMessage
    {
        public string ProductId { get; set; }
        public UserContext UserContext { get; set; }
    }
}

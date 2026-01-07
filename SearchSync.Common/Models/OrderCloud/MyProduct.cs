using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MyProduct : Product<ProductXp> { }

    // Define your custom XP here
    public class ProductXp
    {
        public string ExampleField { get; set; }
    }
}

using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MyBuyer : Buyer<BuyerXp> { }

    public class BuyerXp
    {
        // Define your custom XP here
        public string ExampleField { get; set; }
    }
}

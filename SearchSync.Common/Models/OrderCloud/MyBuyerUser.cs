using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MyBuyerUser : User<BuyerUserXp> { }

    public class BuyerUserXp
    {
        // Define your custom XP here
        public string ExampleField { get; set; }
    }
}

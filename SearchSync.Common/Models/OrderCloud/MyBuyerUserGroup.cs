using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MyBuyerUserGroup : UserGroup<BuyerUserGroupXp> { }

    public class BuyerUserGroupXp
    {
        // Define your custom XP here
        public string ExampleField { get; set; }
    }
}

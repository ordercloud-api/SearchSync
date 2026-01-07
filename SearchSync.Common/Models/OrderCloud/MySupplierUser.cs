using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MySupplierUser : User<SupplierUserXp> { }

    // Define your custom XP here
    public class SupplierUserXp
    {
        public string ExampleField { get; set; }
    }
}

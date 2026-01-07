using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MySupplier : Supplier<SupplierXp> { }

    public class SupplierXp
    {
        // Define your custom XP here
        public string ExampleField { get; set; }
    }
}

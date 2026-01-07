using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MyAdminUser : User<AdminUserXp> { }

    public class AdminUserXp
    {
        // Define your custom XP here
        public string ExampleField { get; set; }
    }
}

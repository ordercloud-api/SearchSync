using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MyCategory : Category<CategoryXp> { }

    // Define your custom XP here
    public class CategoryXp
    {
        public string ExampleField { get; set; }
    }
}

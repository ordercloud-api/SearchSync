using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MyPriceSchedule : PriceSchedule<PriceScheduleXp> { }

    // Define your custom XP here
    public class PriceScheduleXp
    {
        public string ExampleField { get; set; }
    }
}

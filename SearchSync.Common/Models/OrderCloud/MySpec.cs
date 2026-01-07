using OrderCloud.SDK;

namespace SearchSync.Common.Models.OrderCloud
{
    public class MySpec : Spec<SpecXp, MySpecOption> { }

    public class SpecXp
    {
        // Define your custom XP here
        public string ExampleField { get; set; }
    }

    public class MySpecOption : SpecOption<SpecOptionXp> { }


    public class SpecOptionXp
    {
        // Define your custom XP here
        public string ExampleField { get; set; }
    }
}

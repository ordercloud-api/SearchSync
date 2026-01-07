namespace SearchSync.Common.Models.OrderCloud.Sync
{
    public class UserContext
    {
        // Set if the user is authenticated.
        public string ID { get; set; }

        // Set if the user is anonymous or transitioning from an anonymous user to an authenticated one.
        public string AnonymousId { get; set; }
    }
}

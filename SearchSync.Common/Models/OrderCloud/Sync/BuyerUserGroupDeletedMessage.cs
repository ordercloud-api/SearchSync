namespace SearchSync.Common.Models.OrderCloud.Sync
{
    public class BuyerUserGroupDeletedMessage
    {
        public UserContext UserContext { get; set; }
        public string ID { get; set; }
        public string UserGroupType { get; set; }
    }
}

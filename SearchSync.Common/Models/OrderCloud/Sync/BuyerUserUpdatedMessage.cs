namespace SearchSync.Common.Models.OrderCloud.Sync
{
    public class BuyerUserUpdatedMessage : MyBuyerUser
    {
        public UserContext UserContext { get; set; }
        public int UserType { get; set; } // 0 - Buyer, 1 - Supplier, 2 - Admin
        string CompanyID { get; set; }
    }
}

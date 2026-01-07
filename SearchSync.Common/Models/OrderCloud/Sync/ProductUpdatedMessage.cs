using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Models.OrderCloud
{
    public class ProductUpdatedMessage : MyProduct
    {
        public string ProductID { get; set; }
        public UserContext UserContext { get; set; }
        public string Marketplace { get; set; }
        public MyPriceSchedule DefaultPriceSchedule { get; set; }
        public List<PartyPriceSchedule> PartyPriceSchedules { get; set; }
        public List<SellerDefaultPriceSchedule> SellerDefaultPriceSchedules { get; set; }
        public string[] Catalogs { get; set; }
        public string[] Suppliers { get; set; }
        public string[] Buyers { get; set; }
        public List<ProductsUpdatedEventCategory> Categories { get; set; }
        public string[] UserGroups { get; set; }
        public List<MySpec> Specs { get; set; }
    }

    public class ProductsUpdatedEventCategory
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int ListOrder { get; set; }
        public bool Active { get; set; }
    }

    public class PartyPriceSchedule
    {
        public string Seller { get; set; }
        public string Party { get; set; }

        //Type of the party identifier:
        // 1- User
        // 2- UserGroup
        // 3- Company
        public int PartyType { get; set; }
        public MyPriceSchedule PriceSchedule { get; set; }
    }

    public class SellerDefaultPriceSchedule
    {
        public string Seller { get; set; }
        public MyPriceSchedule PriceSchedule { get; set; }
    }
}

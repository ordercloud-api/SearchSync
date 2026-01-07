namespace SearchSync.Common.Models.OrderCloud.Sync
{
    public class InventoryRecordUpdatedMessage
    {
        public UserContext UserContext { get; set; }
        public string ID { get; set; }
        public string ProductID { get; set; }
        public List<MinimalInventoryRecord> InventoryRecords { get; set; }
    }

    public class MinimalInventoryRecord
    {
        public string ID { get; set; }
        public string OwnerID { get; set; }
        public string AddressID { get; set; }
        public bool OrderCanExceed { get; set; }
        public int QuantityAvailable { get; set; }
        public string Zip { get; set; }
        public InventoryXp xp { get; set; }
    }
}

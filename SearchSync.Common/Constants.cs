namespace SearchSync.Common
{
    public static class Constants
    {
        public static class Events
        {
            public const string ProductDeleted = "sitecore.ordercloud.messages.product.deleted";
            public const string ProductUpdated = "sitecore.ordercloud.messages.product.updated";

            public const string CategoryUpdated = "sitecore.ordercloud.messages.entitysync.category.updated";
            public const string CategoryDeleted = "sitecore.ordercloud.messages.entitysync.category.deleted";

            public const string AdminUserUpdated = "sitecore.ordercloud.messages.entitysync.adminuser.updated";
            public const string AdminUserDeleted = "sitecore.ordercloud.messages.entitysync.adminuser.deleted";

            public const string BuyerDeleted = "sitecore.ordercloud.messages.entitysync.buyer.deleted";
            public const string BuyerUpdated = "sitecore.ordercloud.messages.entitysync.buyer.updated";

            public const string BuyerUserGroupDeleted = "sitecore.ordercloud.messages.entitysync.buyerusergroup.deleted";
            public const string BuyerUserGroupUpdated = "sitecore.ordercloud.messages.entitysync.buyerusergroup.updated";

            public const string BuyerUserUpdated = "sitecore.ordercloud.messages.entitysync.buyeruser.updated";
            public const string BuyerUserDeleted = "sitecore.ordercloud.messages.entitysync.buyeruser.deleted";

            public const string InventoryRecordUpdated = "sitecore.ordercloud.messages.entitysync.inventoryrecord.updated";
            public const string InventoryRecordFullUpdated = "sitecore.ordercloud.messages.entitysync.inventoryrecord.full.updated";

            public const string SupplierUserUpdated = "sitecore.ordercloud.messages.entitysync.supplieruser.updated";
            public const string SupplierUserDeleted = "sitecore.ordercloud.messages.entitysync.supplieruser.deleted";

            public const string SupplierUpdated = "sitecore.ordercloud.messages.entitysync.supplier.updated";
            public const string SupplierDeleted = "sitecore.ordercloud.messages.entitysync.supplier.deleted";
        }


        public static readonly HashSet<string> KnownEventTypes = typeof(Events)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string))
            .Select(f => f.GetValue(null) as string)
            .Where(v => v is not null)
            .ToHashSet()!;
    }
}

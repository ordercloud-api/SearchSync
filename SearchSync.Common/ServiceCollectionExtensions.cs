using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SearchSync.Common.Mappers;
using SearchSync.Common.Models;
using SearchSync.Common.Services;

namespace SearchSync.Common
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures all the necessary SearchSync services to the service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureOptions">An action to configure the unified SearchSync options.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddSearchSync(this IServiceCollection services, Action<SearchSyncOptions> configureOptions)
        {
            services.Configure(configureOptions);

            services.AddOptions<SearchIngestionClientOptions>()
                .Configure<IOptions<SearchSyncOptions>>((clientOptions, syncOptions) =>
                {
                    var settings = syncOptions.Value;
                    clientOptions.EndpointUrl = settings.IngestionEndpointUrl;
                    clientOptions.ApiKey = settings.IngestionApiKey;
                    clientOptions.DomainId = settings.DomainId;
                    clientOptions.SourceId = settings.SourceId;
                });

            services.AddOptions<SearchIngestionServiceOptions>()
                .Configure<IOptions<SearchSyncOptions>>((serviceOptions, syncOptions) =>
                {
                    var settings = syncOptions.Value;
                    serviceOptions.EntityProduct = settings.EntityProduct;
                    serviceOptions.EntityCategory = settings.EntityCategory;
                    serviceOptions.EntityAdminUser = settings.EntityAdminUser;
                    serviceOptions.EntityBuyer = settings.EntityBuyer;
                    serviceOptions.EntityBuyerUserGroup = settings.EntityBuyerUserGroup;
                    serviceOptions.EntityBuyerUser = settings.EntityBuyerUser;
                    serviceOptions.EntityInventoryRecord = settings.EntityInventoryRecord;
                    serviceOptions.EntitySupplier = settings.EntitySupplier;
                    serviceOptions.EntitySupplierUser = settings.EntitySupplierUser;
                });

            // Register core services
            services.AddSingleton<ISearchIngestionClient, SearchIngestionClient>();
            services.AddSingleton<ISearchIngestionService, SearchIngestionService>();
            services.AddSingleton<IEventParser, EventParser>();

            // Register all mappers
            services.AddSingleton<IProductUpdatedMapper, ProductUpdatedMapper>();
            services.AddSingleton<IProductDeletedMapper, ProductDeletedMapper>();
            services.AddSingleton<ICategoryUpdatedMapper, CategoryUpdatedMapper>();
            services.AddSingleton<ICategoryDeletedMapper, CategoryDeletedMapper>();
            services.AddSingleton<IAdminUserUpdatedMapper, AdminUserUpdatedMapper>();
            services.AddSingleton<IAdminUserDeletedMapper, AdminUserDeletedMapper>();
            services.AddSingleton<IBuyerUpdatedMapper, BuyerUpdatedMapper>();
            services.AddSingleton<IBuyerDeletedMapper, BuyerDeletedMapper>();
            services.AddSingleton<IBuyerUserGroupUpdatedMapper, BuyerUserGroupUpdatedMapper>();
            services.AddSingleton<IBuyerUserGroupDeletedMapper, BuyerUserGroupDeletedMapper>();
            services.AddSingleton<IBuyerUserUpdatedMapper, BuyerUserUpdatedMapper>();
            services.AddSingleton<IBuyerUserDeletedMapper, BuyerUserDeletedMapper>();
            services.AddSingleton<IInventoryRecordUpdatedMapper, InventoryRecordUpdatedMapper>();
            services.AddSingleton<IInventoryRecordDeletedMapper, InventoryRecordDeletedMapper>();
            services.AddSingleton<ISupplierUpdatedMapper, SupplierUpdatedMapper>();
            services.AddSingleton<ISupplierDeletedMapper, SupplierDeletedMapper>();
            services.AddSingleton<ISupplierUserUpdatedMapper, SupplierUserUpdatedMapper>();
            services.AddSingleton<ISupplierUserDeletedMapper, SupplierUserDeletedMapper>();

            return services;
        }
    }
}

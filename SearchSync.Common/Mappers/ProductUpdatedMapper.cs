using OrderCloud.SDK;
using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud;

namespace SearchSync.Common.Mappers
{

    public interface IProductUpdatedMapper : ISearchMapper<ProductUpdatedMessage>
    {

    }

    public class ProductUpdatedMapper : SearchMapper<ProductUpdatedMessage>, IProductUpdatedMapper
    {
        public ProductUpdatedMapper()
        {
        }

        public override string GetDocumentId(ProductUpdatedMessage message)
        {
            return message.ProductID;
        }

        public override SearchDocument MapToSearchDocument(ProductUpdatedMessage message)
        {
            var document = new SearchDocument();

            if (message == null)
            {
                throw new MappingException("[ProductUpdated] Message is empty");
            }

            try
            {
                document.DocumentId = message.ProductID;

                // product fields 
                document.AddField("name", message.Name);
                document.AddField("description", message.Description);
                document.AddField("type", "product");
                document.AddField("ownerid", message.OwnerID);
                document.AddField("active", message.Active);

                // Add any custom xp fields here as needed
                if (message.xp != null)
                {
                    document.AddField("xp_examplefield", message.xp.ExampleField);
                }

                // needed for visibility
                document.AddField("marketplace", message.Marketplace);
                document.AddFieldCollection("buyers", message.Buyers);
                document.AddFieldCollection("suppliers", message.Suppliers);
                document.AddFieldCollection("usergroups", message.UserGroups);

                // needed for category/catalog filtering
                document.AddFieldCollection("catalogs", message.Catalogs);
                MapCategoriesToSearchDocument(message, document);


                // needed for pricing

                document.AddField("defaultpricescheduleid", message.DefaultPriceScheduleID);

                if (message.PartyPriceSchedules != null && message.PartyPriceSchedules.Count > 0)
                {
                    var mappedPricescheduleList = message.PartyPriceSchedules.Select(party =>
                    {
                        var mappedPriceSchedule = MapPriceScheduleToSearchDocument(party.PriceSchedule);
                        mappedPriceSchedule.Add("seller", party.Seller);
                        mappedPriceSchedule.Add("party", party.Party);
                        mappedPriceSchedule.Add("partytype", party.PartyType);
                        return mappedPriceSchedule;
                    }).ToList();

                    document.AddField("partypriceschedules", mappedPricescheduleList);
                }

                if (message.SellerDefaultPriceSchedules != null && message.SellerDefaultPriceSchedules.Count > 0)
                {
                    var mappedPricescheduleList = message.SellerDefaultPriceSchedules.Select(party =>
                    {
                        var mappedPriceSchedule = MapPriceScheduleToSearchDocument(party.PriceSchedule);
                        mappedPriceSchedule.Add("seller", party.Seller);
                        return mappedPriceSchedule;
                    }).ToList();

                    document.AddField("sellerdefaultpriceschedules", mappedPricescheduleList);
                }

            }
            catch (Exception ex)
            {
                throw new MappingException(ex, message);
            }

            return document;
        }

        private void MapCategoriesToSearchDocument(ProductUpdatedMessage message, SearchDocument document)
        {
            if (message.Categories != null && message.Categories.Count > 0)
            {
                var allCategories = new List<Dictionary<string, object>>();
                var categoryNames = new List<string>();
                var categoryIds = new List<string>();

                foreach (var category in message.Categories)
                {
                    var addToAllCategories = true;

                    var properties = new Dictionary<string, object>
                    {
                        { "id", category.ID },
                        { "listorder", category.ListOrder }
                    };

                    if (!string.IsNullOrEmpty(category.Name))
                    {
                        properties.Add("name", category.Name);
                    }

                    if (category.Active)
                    {
                        properties.Add("active", category.Active);
                        if (category.Active)
                        {
                            categoryIds.Add(category.ID);
                            if (!string.IsNullOrEmpty(category.Name))
                            {
                                categoryNames.Add(category.Name);
                            }
                        }
                        else
                        {
                            // OCM-214: Do no add inactive categories to the categories property.
                            addToAllCategories = false;
                        }
                    }
                    else
                    {
                        // Before we had the active flags, we used to add all categories.  Keeping previous behavior when no active flag is passed in.
                        categoryIds.Add(category.ID);
                        if (!string.IsNullOrEmpty(category.Name))
                        {
                            categoryNames.Add(category.Name);
                        }
                    }

                    if (addToAllCategories)
                    {
                        allCategories.Add(properties);
                    }
                }

                document.Fields.Add("categories", allCategories);
                document.Fields.Add("ccids", categoryIds);

                if (categoryNames.Count > 0)
                {
                    document.Fields.Add("category_names", categoryNames);
                }
            }
        }

        private Dictionary<string, object> MapPriceScheduleToSearchDocument(MyPriceSchedule priceSchedule)
        {
            var result = new Dictionary<string, object>();

            var priceScheduleDictionary = new Dictionary<string, object>();

            // standard price schedule properties, only adding most commonly used.
            // Add more here if needed for your use case
            priceScheduleDictionary.Add("id", priceSchedule.ID);
            priceScheduleDictionary.Add("ownerid", priceSchedule.OwnerID);
            priceScheduleDictionary.Add("name", priceSchedule.Name);
            priceScheduleDictionary.Add("currency", priceSchedule.Currency);
            priceScheduleDictionary.Add("salestart", priceSchedule.SaleStart);
            priceScheduleDictionary.Add("saleend", priceSchedule.SaleEnd);

            if (priceSchedule.PriceBreaks != null && priceSchedule.PriceBreaks.Count > 0)
            {
                var mappedPriceBreakList = priceSchedule.PriceBreaks?.Select(priceBreak => MapPriceBreakToSearchDocument(priceBreak)).ToList();
                priceScheduleDictionary.Add("pricebreaks", mappedPriceBreakList);
            }

            // Add any custom xp fields here as needed
            if (priceSchedule.xp != null)
            {
                priceScheduleDictionary.Add("xp_examplefield", priceSchedule.xp.ExampleField);
            }

            return priceScheduleDictionary;
        }

        private Dictionary<string, object> MapPriceBreakToSearchDocument(PriceBreak priceBreak)
        {
            var priceBreakDictionary = new Dictionary<string, object>();

            priceBreakDictionary.Add("quantity", priceBreak.Quantity);
            priceBreakDictionary.Add("price", priceBreak.Price);
            priceBreakDictionary.Add("salePrice", priceBreak.SalePrice);

            return priceBreakDictionary;
        }


        public override void Validate(ProductUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[ProductUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ProductID))
            {
                errors.Add(new MappingValidationError("[ProductUpdate] ProductId is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }
    }
}

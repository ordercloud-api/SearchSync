using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IBuyerUserGroupUpdatedMapper : ISearchMapper<BuyerUserGroupUpdatedMessage>
    {
    }

    public class BuyerUserGroupUpdatedMapper : SearchMapper<BuyerUserGroupUpdatedMessage>, IBuyerUserGroupUpdatedMapper
    {
        public override string GetDocumentId(BuyerUserGroupUpdatedMessage message)
        {
            return message.ID;
        }

        public override void Validate(BuyerUserGroupUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[BuyerUserGroupUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[BuyerUserGroupUpdate] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(BuyerUserGroupUpdatedMessage message)
        {
            if (message == null)
            {
                throw new MappingException("[BuyerUserGroupUpdate] Message is empty");
            }

            var document = new SearchDocument
            {
                DocumentId = message.ID
            };

            try
            {
                document.AddField("id", message.ID);
                document.AddField("name", message.Name);

                // Add any custom xp fields here as needed
                if (message.xp != null)
                {
                    document.AddField("xp_somefield", message.xp.ExampleField);
                }
            }
            catch (Exception ex)
            {
                throw new MappingException(ex, message);
            }

            return document;
        }
    }
}

using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IBuyerUserGroupDeletedMapper : ISearchMapper<BuyerUserGroupDeletedMessage>
    {
    }

    public class BuyerUserGroupDeletedMapper : SearchMapper<BuyerUserGroupDeletedMessage>, IBuyerUserGroupDeletedMapper
    {
        public override string GetDocumentId(BuyerUserGroupDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(BuyerUserGroupDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[BuyerUserGroupDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[BuyerUserGroupDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(BuyerUserGroupDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

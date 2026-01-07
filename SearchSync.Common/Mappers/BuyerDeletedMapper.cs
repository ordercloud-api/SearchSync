using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IBuyerDeletedMapper : ISearchMapper<BuyerDeletedMessage>
    {
    }

    public class BuyerDeletedMapper : SearchMapper<BuyerDeletedMessage>, IBuyerDeletedMapper
    {
        public override string GetDocumentId(BuyerDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(BuyerDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[BuyerDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[BuyerDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(BuyerDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

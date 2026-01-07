using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IBuyerUserDeletedMapper : ISearchMapper<UserDeletedMessage>
    {
    }

    public class BuyerUserDeletedMapper : SearchMapper<UserDeletedMessage>, IBuyerUserDeletedMapper
    {
        public override string GetDocumentId(UserDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(UserDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[BuyerUserDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[BuyerUserDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(UserDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

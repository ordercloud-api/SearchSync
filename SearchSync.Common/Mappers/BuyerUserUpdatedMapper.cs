using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IBuyerUserUpdatedMapper : ISearchMapper<BuyerUserUpdatedMessage>
    {
    }

    public class BuyerUserUpdatedMapper : SearchMapper<BuyerUserUpdatedMessage>, IBuyerUserUpdatedMapper
    {
        public override string GetDocumentId(BuyerUserUpdatedMessage message)
        {
            return message.ID;
        }

        public override void Validate(BuyerUserUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[BuyerUserUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[BuyerUserUpdate] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(BuyerUserUpdatedMessage message)
        {
            if (message == null)
            {
                throw new MappingException("[BuyerUserUpdate] Message is empty");
            }

            var document = new SearchDocument
            {
                DocumentId = message.ID
            };

            try
            {
                document.AddField("username", message.Username);
                document.AddField("firstname", message.FirstName);
                document.AddField("lastname", message.LastName);
                document.AddField("email", message.Email);
                document.AddField("phone", message.Phone);
                document.AddField("active", message.Active);
                document.AddField("usertype", "Buyer");

                if (message.xp != null)
                {
                    document.AddField("xp_examplefield", message.xp.ExampleField);
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

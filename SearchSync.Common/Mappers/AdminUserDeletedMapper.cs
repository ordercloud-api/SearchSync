using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IAdminUserDeletedMapper : ISearchMapper<AdminUserDeletedMessage>
    {
    }

    public class AdminUserDeletedMapper : SearchMapper<AdminUserDeletedMessage>, IAdminUserDeletedMapper
    {
        public override string GetDocumentId(AdminUserDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(AdminUserDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[AdminUserDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[AdminUserDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(AdminUserDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}

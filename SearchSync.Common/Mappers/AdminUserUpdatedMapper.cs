using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface IAdminUserUpdatedMapper : ISearchMapper<AdminUserUpdatedMessage>
    {
    }

    public class AdminUserUpdatedMapper : SearchMapper<AdminUserUpdatedMessage>, IAdminUserUpdatedMapper
    {
        public override string GetDocumentId(AdminUserUpdatedMessage message)
        {
            return message.ID;
        }

        public override void Validate(AdminUserUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[AdminUserUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[AdminUserUpdate] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(AdminUserUpdatedMessage message)
        {
            if (message == null)
            {
                throw new MappingException("[AdminUserUpdate] Message is empty");
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
                document.AddField("usertype", "Admin");

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

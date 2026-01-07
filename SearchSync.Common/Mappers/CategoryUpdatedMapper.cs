using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface ICategoryUpdatedMapper : ISearchMapper<CategoryUpdatedMessage>
    {
    }

    public class CategoryUpdatedMapper : SearchMapper<CategoryUpdatedMessage>, ICategoryUpdatedMapper
    {
        public override string GetDocumentId(CategoryUpdatedMessage message)
        {
            return message.ID;
        }

        public override void Validate(CategoryUpdatedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[CategoryUpdate] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[CategoryUpdate] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(CategoryUpdatedMessage message)
        {
            if (message == null)
            {
                throw new MappingException("[CategoryUpdate] Message is empty");
            }

            var document = new SearchDocument
            {
                DocumentId = message.ID
            };

            try
            {
                // REQUIRED by Sitecore Search Category entity template
                document.AddField("ccid", message.ID);
                document.AddField("url_path", message.ParentID == null
                    ? $"/{message.ID}"
                    : $"/{message.ParentID}/{message.ID}");

                document.AddField("name", message.Name);
                document.AddField("description", message.Description);
                document.AddField("active", message.Active);
                document.AddField("parentid", message.ParentID);

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
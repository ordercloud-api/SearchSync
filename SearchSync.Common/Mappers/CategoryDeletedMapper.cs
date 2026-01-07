using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.Sync;

namespace SearchSync.Common.Mappers
{
    public interface ICategoryDeletedMapper : ISearchMapper<CategoryDeletedMessage>
    {
    }

    public class CategoryDeletedMapper : SearchMapper<CategoryDeletedMessage>, ICategoryDeletedMapper
    {
        public override string GetDocumentId(CategoryDeletedMessage message)
        {
            return message.ID;
        }

        public override void Validate(CategoryDeletedMessage message)
        {
            var errors = new List<MappingValidationError>();

            if (message == null)
            {
                errors.Add(new MappingValidationError("[CategoryDelete] Message is empty"));
                throw new MappingValidationException(errors);
            }

            if (string.IsNullOrEmpty(message.ID))
            {
                errors.Add(new MappingValidationError("[CategoryDelete] ID is required"));
            }

            if (errors.Any())
            {
                throw new MappingValidationException(errors);
            }
        }

        public override SearchDocument MapToSearchDocument(CategoryDeletedMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
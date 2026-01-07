using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;
using SearchSync.Common.Models.OrderCloud.ProductSync;

namespace SearchSync.Common.Mappers
{
    public interface IProductDeletedMapper : ISearchMapper<ProductDeletedMessage>
    {

    }

    public class ProductDeletedMapper : SearchMapper<ProductDeletedMessage>, IProductDeletedMapper
    {
        public ProductDeletedMapper()
        {

        }

        public override string GetDocumentId(ProductDeletedMessage message)
        {
            return message.ProductId;
        }

        public override SearchDocument MapToSearchDocument(ProductDeletedMessage message)
        {
            throw new NotImplementedException();
        }

        public override void Validate(ProductDeletedMessage message)
        {
            if (message.ProductId == null)
            {
                throw new MappingValidationException("[ProductDelete] ProductId is required");
            }
        }
    }
}

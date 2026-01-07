namespace SearchSync.Common.Models
{
    public interface ISearchMapper<T>
    {
        abstract string GetDocumentId(T message);

        void Validate(T message);

        SearchDocument MapToSearchDocument(T message);
    }

    public abstract class SearchMapper<T> : ISearchMapper<T>
    {
        public abstract void Validate(T message);

        public abstract SearchDocument MapToSearchDocument(T message);

        abstract public string GetDocumentId(T message);
    }
}

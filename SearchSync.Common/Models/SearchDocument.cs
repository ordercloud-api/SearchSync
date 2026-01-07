using Newtonsoft.Json;

namespace SearchSync.Common.Models
{
    public class SearchDocument
    {
        [JsonProperty("id")]
        public string DocumentId { get; set; } = string.Empty;

        [JsonProperty("fields")]
        public Dictionary<string, object> Fields { get; set; } = new();

        public void AddField(string field, object value)
        {
            Fields.Add(field, value);
        }

        public T? GetField<T>(string fieldName)
        {
            Fields.TryGetValue(fieldName, out var field);
            return field == null ? default(T) : (T)field;
        }

        public void AddFieldCollection(string field, IList<string>? value)
        {
            if (value != null && value.Count > 0)
            {
                Fields.Add(field, value);
            }
        }
    }
}

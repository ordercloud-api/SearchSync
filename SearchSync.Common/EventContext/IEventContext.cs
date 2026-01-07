public interface IEventContext
{
    string Body { get; }
    string? GetProperty(string key);
}
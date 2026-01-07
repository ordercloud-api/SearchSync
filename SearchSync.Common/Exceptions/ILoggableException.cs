public interface ILoggableException
{
    string GetLogMessage();
    Dictionary<string, object> GetCustomDimensions();
}
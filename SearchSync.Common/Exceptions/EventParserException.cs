namespace SearchSync.Common.Exceptions
{
    public class EventParserException : Exception, ILoggableException
    {
        public EventParserException(string message) : base(message)
        {

        }

        public EventParserException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public string GetLogMessage()
        {
            return Message;
        }

        public Dictionary<string, object> GetCustomDimensions() => new Dictionary<string, object>() { };
    }
}

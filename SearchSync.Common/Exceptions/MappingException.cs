namespace SearchSync.Common.Exceptions
{
    public class MappingException : Exception, ILoggableException
    {
        public readonly object? EventMessage;

        public MappingException(string message) : base(message)
        {

        }

        public MappingException(string message, Exception innerException, object? eventMessage) : base(message, innerException)
        {
            EventMessage = eventMessage;
        }

        public MappingException(Exception innerException, object? eventMessage) : base("An unknown error occurred while mapping", innerException)
        {
            EventMessage = eventMessage;
        }

        public string GetLogMessage()
        {
            return Message;
        }

        public Dictionary<string, object> GetCustomDimensions()
        {
            return new Dictionary<string, object>()
            {
                ["EventMessage"] = EventMessage
            };
        }
    }
}

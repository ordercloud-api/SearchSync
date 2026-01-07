namespace SearchSync.Common.Exceptions
{
    public class MappingValidationError
    {
        public readonly string Message;
        public readonly object? Data;

        public MappingValidationError(string message)
        {
            Message = message;
        }

        public MappingValidationError(string message, object? data)
        {
            Message = message;
            Data = data;
        }
    }

    public class MappingValidationException : Exception, ILoggableException
    {
        public readonly List<MappingValidationError> Errors;

        public MappingValidationException(string message) : base(message)
        {
            var errors = new List<MappingValidationError>();
            errors.Add(new MappingValidationError(message));
            Errors = errors;
        }

        public MappingValidationException(List<MappingValidationError> errors) : base(string.Join('|', errors.Select(e => e.Message)))
        {
            Errors = errors;
        }

        public string GetLogMessage()
        {
            return Message;
        }

        public Dictionary<string, object> GetCustomDimensions()
        {
            return new Dictionary<string, object>()
            {
                ["Errors"] = Errors
            };
        }
    }
}

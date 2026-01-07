using Flurl.Http;

namespace SearchSync.Common.Exceptions
{
    public class SearchRequestException : Exception, ILoggableException
    {
        public int? StatusCode { get; }
        public string? RequestUrl { get; }
        public string? RequestBody { get; }
        public string? ResponseBody { get; }

        public SearchRequestException(FlurlHttpException ex) : base($"Search request failed")
        {
            StatusCode = ex.Call?.Response?.StatusCode;
            RequestUrl = ex.Call?.Request?.Url.ToString();
            RequestBody = ex.Call?.RequestBody;
            ResponseBody = ex.GetResponseStringAsync().GetAwaiter().GetResult();
        }

        public string GetLogMessage()
        {
            return $"SearchRequestException: StatusCode={StatusCode}, RequestUrl={RequestUrl}, RequestBody={RequestBody}, ResponseBody={ResponseBody}";
        }

        public Dictionary<string, object> GetCustomDimensions()
        {
            return new Dictionary<string, object>()
            {
                ["StatusCode"] = StatusCode,
                ["RequestUrl"] = RequestUrl,
                ["RequestBody"] = RequestBody,
                ["ResponseBody"] = ResponseBody
            };
        }
    }
}

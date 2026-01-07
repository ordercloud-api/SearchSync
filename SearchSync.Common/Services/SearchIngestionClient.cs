using Flurl.Http;
using Flurl.Http.Newtonsoft;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Retry;
using SearchSync.Common.Exceptions;
using SearchSync.Common.Models;

namespace SearchSync.Common.Services
{
    public interface ISearchIngestionClient
    {
        Task<IFlurlResponse> PutAsync(object payload, params string[] segments);
        Task<IFlurlResponse> PatchAsync(object payload, params string[] segments);
        Task<IFlurlResponse> DeleteAsync(params string[] segments);
    }

    public class SearchIngestionClient : ISearchIngestionClient
    {
        private readonly IFlurlClient _client;
        private readonly AsyncRetryPolicy<IFlurlResponse> _retryPolicy;
        private readonly ILogger<SearchIngestionClient> _logger;

        public SearchIngestionClient(IOptions<SearchIngestionClientOptions> options, ILogger<SearchIngestionClient> logger)
        {
            _logger = logger;

            var settings = options.Value;
            var apiKey = settings.ApiKey;
            var domainId = settings.DomainId;
            var sourceId = settings.SourceId;
            var fullUrl = settings.EndpointUrl.TrimEnd('/') + $"/domains/{domainId}/sources/{sourceId}/entities";

            _client = new FlurlClient(fullUrl)
                .WithHeader("Authorization", apiKey)
                .WithSettings(settings =>
                {
                    settings.JsonSerializer = new NewtonsoftJsonSerializer();
                });

            // https://github.com/Polly-Contrib/Polly.Contrib.WaitAndRetry#wait-and-retry-with-jittered-back-off
            // approximate delays: 1s, 2s, 4s, 8s, 16s. so a little over 30s total if the 5th one is hit
            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

            _retryPolicy = Policy
                .Handle<FlurlHttpException>(e => e.StatusCode == 429)
                .OrResult<IFlurlResponse>(r => r.StatusCode == 429)
                .WaitAndRetryAsync(
                    delay,
                    onRetry: (result, wait, retryAttempt, _) =>
                    {
                        _logger.LogWarning($"Retry {retryAttempt} after {wait.TotalSeconds}s due to 429.");
                    }
                );
        }

        public Task<IFlurlResponse> PutAsync(object payload, params string[] segments) =>
            SendAsync(HttpMethod.Put, payload, segments);

        public Task<IFlurlResponse> PatchAsync(object payload, params string[] segments) =>
            SendAsync(HttpMethod.Patch, payload, segments);

        public Task<IFlurlResponse> DeleteAsync(params string[] segments) =>
            SendAsync(HttpMethod.Delete, null, segments);

        private object WrapInDocument(object payload)
        {
            return new { document = payload };
        }

        private async Task<IFlurlResponse> SendAsync(HttpMethod method, object? payload = null, params string[] segments)
        {
            _logger.LogDebug("Sending search request");
            var request = _client.Request(segments).SetQueryParam("locale", "en_us");

            if (payload != null)
            {
                payload = WrapInDocument(payload);
            }

            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    switch (method.Method)
                    {
                        case "PUT":
                            return await request.PutJsonAsync(payload);
                        case "PATCH":
                            return await request.PatchJsonAsync(payload);
                        case "DELETE":
                            return await request.DeleteAsync();
                        default:
                            throw new NotSupportedException($"HTTP method {method} not supported.");
                    }
                });
            }
            catch (FlurlHttpException ex)
            {
                throw new SearchRequestException(ex);
            }
        }
    }
}

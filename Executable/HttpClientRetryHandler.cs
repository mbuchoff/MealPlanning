using System.Net;

namespace SystemOfEquations;

public class HttpClientRetryHandler : DelegatingHandler
{
    private const int MaxRetries = 3;
    private readonly Action<string>? _onRetry;

    public HttpClientRetryHandler() : base(new HttpClientHandler())
    { }

    public HttpClientRetryHandler(HttpMessageHandler innerHandler, Action<string>? onRetry = null) : base(innerHandler)
    {
        _onRetry = onRetry;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;
        int attempt = 0;

        while (true)
        {
            attempt++;
            response = await base.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            // If rate limited (429), wait and retry indefinitely
            if (response.StatusCode == HttpStatusCode.TooManyRequests)
            {
                _onRetry?.Invoke("Rate limit hit, waiting before retry...");
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                continue;
            }

            // For other errors, give up after MaxRetries
            if (attempt >= MaxRetries)
            {
                return response;
            }
        }
    }
}

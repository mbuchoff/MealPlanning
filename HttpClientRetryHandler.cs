namespace SystemOfEquations;

public class HttpClientRetryHandler : DelegatingHandler
{
    private const int MaxRetries = 3;

    public HttpClientRetryHandler() : base(new HttpClientHandler())
    { }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        HttpResponseMessage? response = null;
        for (int i = 0; i < MaxRetries; i++)
        {
            response = await base.SendAsync(request, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return response;
            }
        }

        return response ?? throw new Exception($"{nameof(response)} was never asigned to. This shouldn't be possible.");
    }
}

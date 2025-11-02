using System.Net;
using SystemOfEquations;

namespace Test;

public class HttpClientRetryHandlerTests
{
    [Fact]
    public async Task SendAsync_Should_Retry_Indefinitely_On_429()
    {
        // Arrange
        var attemptCount = 0;
        var testHandler = new TestHttpMessageHandler(request =>
        {
            attemptCount++;
            // Fail 10 times with 429, then succeed (proves it keeps retrying)
            if (attemptCount <= 10)
            {
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var retryHandler = new HttpClientRetryHandler(testHandler);
        var client = new HttpClient(retryHandler);

        var startTime = DateTimeOffset.UtcNow;

        // Act
        var response = await client.GetAsync("http://example.com");

        var elapsed = DateTimeOffset.UtcNow - startTime;

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(11, attemptCount); // Should have retried 10 times, succeeded on 11th

        // Should have waited at least 10 seconds total (10 retries * ~1 second each)
        Assert.True(elapsed.TotalSeconds >= 10, $"Expected at least 10 seconds delay, but was {elapsed.TotalSeconds:F2}s");
    }

    [Fact]
    public async Task SendAsync_Should_Call_OnRetry_When_429_Occurs()
    {
        // Arrange
        var retryMessages = new List<string>();
        var attemptCount = 0;
        var testHandler = new TestHttpMessageHandler(request =>
        {
            attemptCount++;
            if (attemptCount < 2)
            {
                return new HttpResponseMessage(HttpStatusCode.TooManyRequests);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var retryHandler = new HttpClientRetryHandler(testHandler, message => retryMessages.Add(message));
        var client = new HttpClient(retryHandler);

        // Act
        await client.GetAsync("http://example.com");

        // Assert
        Assert.Single(retryMessages);
        Assert.Contains("rate limit", retryMessages[0], StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task SendAsync_Should_Give_Up_After_3_Attempts_On_Non_429_Errors()
    {
        // Arrange
        var attemptCount = 0;
        var testHandler = new TestHttpMessageHandler(request =>
        {
            attemptCount++;
            // Always fail with 500 - should give up after 3 attempts
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        });

        var retryHandler = new HttpClientRetryHandler(testHandler);
        var client = new HttpClient(retryHandler);

        var startTime = DateTimeOffset.UtcNow;

        // Act
        var response = await client.GetAsync("http://example.com");

        var elapsed = DateTimeOffset.UtcNow - startTime;

        // Assert
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal(3, attemptCount); // Should give up after 3 attempts

        // Should NOT have delayed (immediate retries for non-429)
        Assert.True(elapsed.TotalSeconds < 1, $"Expected less than 1 second (no delay), but was {elapsed.TotalSeconds:F2}s");
    }

    private class TestHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public TestHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }
}

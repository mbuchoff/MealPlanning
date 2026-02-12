using System.Net;
using System.Text;
using System.Text.Json;
using SystemOfEquations.Todoist;

namespace SystemOfEquations.Test;

[Collection("TodoistApi")]
public class TodoistApiConcurrencyTests : IDisposable
{
    public void Dispose()
    {
        TodoistApi.ResetHttpOverridesForTests();
    }

    [Fact]
    public async Task GetProjectsAsync_Should_Limit_Concurrent_Requests()
    {
        var current = 0;
        var max = 0;

        var handler = new TestHttpMessageHandler(async request =>
        {
            var now = Interlocked.Increment(ref current);
            InterlockedExtensions.UpdateMax(ref max, now);

            try
            {
                // Keep the request "in flight" briefly so concurrency can build up.
                await Task.Delay(50);

                Assert.Equal("https://api.todoist.com/api/v1/projects", request.RequestUri?.ToString());

                var body = JsonSerializer.Serialize(new { results = Array.Empty<object>() });
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(body, Encoding.UTF8, "application/json")
                };
            }
            finally
            {
                Interlocked.Decrement(ref current);
            }
        });

        TodoistApi.SetHttpOverridesForTests("test-api-key", handler);

        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => TodoistApi.GetProjectsAsync()));

        Assert.True(max <= 4, $"Expected at most 4 concurrent requests, but saw {max}.");
    }

    private sealed class TestHttpMessageHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return handler(request);
        }
    }

    private static class InterlockedExtensions
    {
        public static void UpdateMax(ref int target, int value)
        {
            while (true)
            {
                var snapshot = Volatile.Read(ref target);
                if (value <= snapshot)
                    return;

                if (Interlocked.CompareExchange(ref target, value, snapshot) == snapshot)
                    return;
            }
        }
    }
}

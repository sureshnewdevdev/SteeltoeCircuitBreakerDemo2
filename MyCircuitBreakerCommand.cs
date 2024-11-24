using Steeltoe.CircuitBreaker.Hystrix;

public class MyCircuitBreakerCommand : HystrixCommand<string>
{
    private readonly ILogger<MyCircuitBreakerCommand> _logger;
    private static int _failureCount = 0;
    private readonly HttpClient _httpClient;

    public MyCircuitBreakerCommand(IHystrixCommandOptions options,
        ILogger<MyCircuitBreakerCommand> logger,
        IHttpClientFactory httpClientFactory) : base(options)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    protected override async Task<string> RunAsync()
    {
        _logger.LogInformation($"Attempt {_failureCount + 1}");

        // Simulate a failing service
        if (_failureCount++ % 2 == 0)
        {
            _logger.LogWarning("Simulating a failure");
            throw new Exception("Service unavailable");
        }

        // Simulate successful service call
        await Task.Delay(100); // Simulate some work
        return $"Success! Attempt: {_failureCount}";
    }

    protected override async Task<string> RunFallbackAsync()
    {
        _logger.LogInformation("Executing fallback");
        return $"Fallback response (Circuit is {(IsCircuitBreakerOpen ? "OPEN" : "CLOSED")})";
    }
}
using Microsoft.AspNetCore.Mvc;
using Steeltoe.CircuitBreaker.Hystrix;

[ApiController]
[Route("[controller]")]
public class CircuitBreakerTestController : ControllerBase
{
    private readonly MyCircuitBreakerCommand _circuitBreaker;
    private readonly ILogger<CircuitBreakerTestController> _logger;
    private static int _failureCount = 0;

    public CircuitBreakerTestController(MyCircuitBreakerCommand circuitBreaker,
        ILogger<CircuitBreakerTestController> logger)
    {
        _circuitBreaker = circuitBreaker;
        _logger = logger;
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        try
        {
            var result = await _circuitBreaker.ExecuteAsync();
            return Ok(new
            {
                Result = result,
                CircuitState = GetCircuitState()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = ex.Message,
                CircuitState = GetCircuitState()
            });
        }
    }

    private string GetCircuitState()
    {
        if (_circuitBreaker.IsCircuitBreakerOpen)
            return "OPEN";
        return "CLOSED";
    }

    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new
        {
            CircuitState = GetCircuitState(),
            Metrics = new
            {
                //TotalRequests = _circuitBreaker.Metrics.GetHealthCounts().TotalRequests,
                //ErrorPercentage = _circuitBreaker.Metrics.GetHealthCounts().ErrorPercentage,
                SuccessCount = _circuitBreaker.Metrics.GetCumulativeCount(HystrixEventType.SUCCESS),
                FailureCount = _circuitBreaker.Metrics.GetCumulativeCount(HystrixEventType.FAILURE),
                TimeoutCount = _circuitBreaker.Metrics.GetCumulativeCount(HystrixEventType.TIMEOUT)
            }
        });
    }

    [HttpGet("force-error")]
    public async Task<IActionResult> ForceError()
    {
        try
        {
            throw new Exception("Forced error");
        }
        catch
        {
            var result = await _circuitBreaker.ExecuteAsync();
            return Ok(new
            {
                Result = result,
                CircuitState = GetCircuitState()
            });
        }
    }
}
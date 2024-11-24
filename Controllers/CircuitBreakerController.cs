using Microsoft.AspNetCore.Mvc;
using Steeltoe.CircuitBreaker.Hystrix;
using SteeltoeCircuitBreakerDemo;

namespace SteeltoeCircuitBreakerDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CircuitBreakerController : ControllerBase
    {
        private readonly MyCircuitBreakerCommand _circuitBreakerCommand;

        public CircuitBreakerController(MyCircuitBreakerCommand circuitBreakerCommand)
        {
            _circuitBreakerCommand = circuitBreakerCommand;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _circuitBreakerCommand.ExecuteAsync();
            return Ok(result);
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Steeltoe.CircuitBreaker.Hystrix;

var builder = WebApplication.CreateBuilder(args);

// Add this line
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add controllers
builder.Services.AddControllers();

// Create Hystrix command keys - fixed the order for the HystrixCommandOptions constructor
var groupKey = HystrixCommandGroupKeyDefault.AsKey("MyCircuitBreakerGroup");
var commandKey = HystrixCommandKeyDefault.AsKey("MyCircuitBreakerCommand");

// Configure Hystrix options programmatically - fixed constructor parameter order
var hystrixOptions = new HystrixCommandOptions(groupKey, commandKey)  // Note: groupKey first, then commandKey
{
    CircuitBreakerEnabled = true,
    CircuitBreakerSleepWindowInMilliseconds = 5000, // Time before retry
    CircuitBreakerRequestVolumeThreshold = 3,       // Minimum requests to trigger
    CircuitBreakerErrorThresholdPercentage = 50,    // Error threshold percentage
    ExecutionTimeoutInMilliseconds = 1000           // Execution timeout
};

// Register HystrixCommandOptions in the service collection
builder.Services.AddSingleton<IHystrixCommandOptions>(hystrixOptions);

// Add Hystrix Command with pre-configured options - fixed configuration
builder.Services.AddHystrixCommand<MyCircuitBreakerCommand>(
    groupKey,  // Use groupKey
    builder.Configuration  // Pass the IConfiguration directly
);

// Add HttpClient for external calls
builder.Services.AddHttpClient();

var app = builder.Build();
// Add these lines
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Add this line to serve static files
app.UseRouting();
 
app.MapControllers();
app.Run();
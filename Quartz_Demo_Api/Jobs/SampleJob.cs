namespace Quartz_Demo_Api.Jobs;

using Quartz;

public class SampleJob : IJob
{
    private readonly ILogger<SampleJob> _logger;

    public SampleJob(
        ILogger<SampleJob> logger
    )
    {
        _logger = logger;
    }

    public Task Execute(IJobExecutionContext context)
    {
        Console.WriteLine("Hello from Quartz!");
        
        _logger.LogInformation("Hello from Quartz!");

        return Task.CompletedTask;
    }
}
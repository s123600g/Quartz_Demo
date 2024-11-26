namespace Quartz_Demo_Api.Listener;

using Quartz;

public class MyJobListener : IJobListener
{
    public string Name => "MyJobListener";

    private readonly ILogger<MyJobListener> _logger;

    public MyJobListener(
        ILogger<MyJobListener> logger
    )
    {
        _logger = logger;
    }

    public async Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        Console.WriteLine($"[Job Listener] Job {context.JobDetail.Key} is about to execute.");

        _logger.LogInformation($"[Job Listener] Job {context.JobDetail.Key} is about to execute.");

        await Task.CompletedTask;
    }

    public async Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        Console.WriteLine($"[Job Listener] Job {context.JobDetail.Key} was executed.");

        await Task.CompletedTask;
    }

    public async Task JobWasExecuted(IJobExecutionContext context, JobExecutionException? jobException, CancellationToken cancellationToken = new CancellationToken())
    {
        Console.WriteLine($"[Job Listener] Job {context.JobDetail.Key} execution was vetoed.");

        _logger.LogInformation($"[Job Listener] Job {context.JobDetail.Key} execution was vetoed.");

        await Task.CompletedTask;
    }
}
namespace Quartz_Demo_Api.Controller;

using Microsoft.AspNetCore.Mvc;
using Quartz_Demo_Api.Jobs;
using Quartz_Demo_Api.Model.Rq;
using Quartz;
using Quartz.Impl.Matchers;

[ApiController]
[Route("[controller]/[action]")]
public class JobManagementController : ControllerBase
{
    private readonly IScheduler _scheduler;

    public JobManagementController(
        ISchedulerFactory schedulerFactory
    )
    {
        _scheduler = schedulerFactory.GetScheduler().GetAwaiter().GetResult();
    }

    private string _getDefaultGroupName => "SampleGroup";
    private string GenTriggerName(string jobKey) => $"{jobKey}-Trigger";

    [HttpPost]
    public async Task<IActionResult> AddJob(
        [FromBody] AddJobRq rq
    )
    {
        var jobKey = new JobKey(
            name: rq.JobKey
            , group: _getDefaultGroupName
        );

        var triggerKey = new TriggerKey(
            name: GenTriggerName(rq.JobKey)
            , group: _getDefaultGroupName
        );

        var job = JobBuilder.Create<SampleJob>()
                            .WithDescription("Sample Job Test")
                            .WithIdentity(jobKey)
                            // .StoreDurably(durability: true)
                            .Build();

        var trigger = TriggerBuilder.Create()
                                    .WithIdentity(triggerKey)
                                    .WithDescription("Sample Trigger Test")
                                    .StartAt(DateTime.Now.AddMinutes(1))
                                    .Build();

        await _scheduler.ScheduleJob(job, trigger);

        return Ok();
    }

    [HttpGet]
    public async Task<ActionResult<QueryJobsRs>> QueryJobs()
    {
        var jobKeys = await _scheduler.GetJobKeys(GroupMatcher<JobKey>.AnyGroup());

        var result = new QueryJobsRs();
        result.SchedulerName = _scheduler.SchedulerName;

        var data = new List<ScheduleDetail>();

        foreach (var jobKey in jobKeys)
        {
            var jobDetail = await _scheduler.GetJobDetail(
                jobKey: jobKey
            );

            if (
                jobDetail != null
            )
            {
                var tempData = new ScheduleDetail();

                tempData.Job = new JobDetail()
                {
                    Name = jobDetail.Key.Name,
                    Group = jobDetail.Key.Group,
                    JobTypeFullName = jobDetail.JobType.FullName
                };

                var triggerKey = new TriggerKey(
                    name: GenTriggerName(jobKey.Name)
                    , group: _getDefaultGroupName
                );

                var triggerDetail = await _scheduler.GetTrigger(
                    triggerKey: triggerKey
                );

                var currentTriggerState = await _scheduler.GetTriggerState(
                    triggerKey: triggerKey
                );
                
                var t = await _scheduler.GetTriggersOfJob(
                    jobKey: jobKey
                );

                if (
                    triggerDetail != null
                )
                {
                    tempData.Trigger = new TriggerDetail()
                    {
                        Name = triggerDetail.Key.Name,
                        Group = triggerDetail.Key.Group,
                        TriggerTypeName = triggerDetail.GetType().Name,
                        Description = triggerDetail.Description,
                        Priority = triggerDetail.Priority,
                        StartTime = triggerDetail.StartTimeUtc.DateTime,
                        EndTime = triggerDetail.EndTimeUtc?.DateTime,
                        NextFireTime = triggerDetail.GetNextFireTimeUtc()?.DateTime,
                        PreviousFireTime = triggerDetail.GetPreviousFireTimeUtc()?.DateTime,
                        State = currentTriggerState,
                    };
                }

                data.Add(tempData);
            }
        }

        result.Data = data;

        return result;
    }

    [HttpPost]
    public async Task<IActionResult> DeleteJob(
        [FromBody] DeleteJobRq rq
    )
    {
        var jobKey = new JobKey(
            name: rq.JobKey
            , group: _getDefaultGroupName
        );

        await _scheduler.DeleteJob(jobKey);

        return Ok("Deleted done.");
    }
}
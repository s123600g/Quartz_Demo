namespace Quartz_Demo_Api.Model.Rq;

using Quartz;

public class QueryJobsRs
{
    public string SchedulerName { get; set; }

    public List<ScheduleDetail> Data { get; set; }
}

public class ScheduleDetail
{
    public JobDetail Job { get; set; }

    public TriggerDetail? Trigger { get; set; }
}

public class JobDetail
{
    public string Name { get; set; }

    public string Group { get; set; }

    public string? JobTypeFullName { get; set; }
}

public class TriggerDetail
{
    public string Name { get; set; }

    public string Group { get; set; }

    public string TriggerTypeName { get; set; }

    public string? Description { get; set; }

    public int Priority { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? NextFireTime { get; set; }

    public DateTime? PreviousFireTime { get; set; }
    
    public TriggerState State { get; set; }
}
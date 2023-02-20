using Quartz;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public class QuartzTriggerListener : ITriggerListener
{
    private readonly ILogger<QuartzTriggerListener> _logger;

    public string Name { get; } = "Quartz Trigger Listener";

    public QuartzTriggerListener(ILogger<QuartzTriggerListener> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task TriggerFired(ITrigger trigger, IJobExecutionContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Trigger fired {jobKey}",
            trigger.JobKey.Name);
        return Task.CompletedTask;
    }

    public Task<bool> VetoJobExecution(ITrigger trigger, IJobExecutionContext context,
        CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Veto check for trigger {jobKey}",
            trigger.JobKey.Name);

        var fireTime = context.ScheduledFireTimeUtc;
        //if (lastFireTime != null && fireTime.equals(lastFireTime))
        //{
        //    return Task.FromResult(true);
        //}
        //lastFireTime = fireTime;

        return Task.FromResult(false);
    }

    public Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = new CancellationToken())
    {
        _logger.LogInformation("Trigger misfired {jobKey}",
            trigger.JobKey.Name);
        return Task.CompletedTask;
    }

    public Task TriggerComplete(ITrigger trigger, IJobExecutionContext context, SchedulerInstruction triggerInstructionCode,
        CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.CompletedTask;
    }
}

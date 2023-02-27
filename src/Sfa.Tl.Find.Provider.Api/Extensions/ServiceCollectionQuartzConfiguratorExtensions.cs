using Quartz;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class ServiceCollectionQuartzConfiguratorExtensions
{
    public static void AddJobAndTrigger<T>(
        this IServiceCollectionQuartzConfigurator quartz,
        string jobName,
        string cronSchedule,
        int? misfireInstruction = null,
        IDictionary<string, string> jobDataList = null)
        where T : IJob
    {
        if (string.IsNullOrEmpty(cronSchedule))
        {
            return;
        }

        var jobKey = new JobKey(jobName);
        quartz.AddJob<T>(opts =>
        {
            opts.WithIdentity(jobKey);
            if (jobDataList != null && jobDataList.Any())
            {
                foreach (var jobData in jobDataList)
                {
                    opts.UsingJobData(jobData.Key, jobData.Value);
                }
            }
        });

        quartz.AddTrigger(opts => opts
            .ForJob(jobKey)
            .WithIdentity($"{jobName}-trigger")
            .WithCronSchedule(cronSchedule, b =>
            {
                switch (misfireInstruction)
                {
                    case MisfireInstruction.CronTrigger.DoNothing:
                        b.WithMisfireHandlingInstructionDoNothing();
                        break;
                    case MisfireInstruction.CronTrigger.FireOnceNow:
                        b.WithMisfireHandlingInstructionFireAndProceed();
                        break;
                }
            }));
    }
}

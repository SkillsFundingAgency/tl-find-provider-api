using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Jobs;

public class ProviderNotificationEmailJobTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(ProviderNotificationEmailJob)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(ProviderNotificationEmailJob)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task Execute_Job_Calls_Expected_Services()
    {
        var providerDataService = Substitute.For<IProviderDataService>();

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey(JobKeys.ProviderNotificationEmail));
        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.Trigger.Returns(trigger);

        var job = new ProviderNotificationEmailJobBuilder()
            .Build(providerDataService);

        await job.Execute(jobContext);

        await providerDataService.Received(1).SendProviderNotifications();
    }
}
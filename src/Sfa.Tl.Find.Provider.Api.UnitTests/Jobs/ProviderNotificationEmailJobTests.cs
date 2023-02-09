using Microsoft.Extensions.Logging;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
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
        const NotificationFrequency frequency = NotificationFrequency.Daily;

        var jobDataMap = new JobDataMap
        {
            { JobDataKeys.NotificationFrequency, frequency.ToString() }
        };
        var jobDetail = Substitute.For<IJobDetail>();
        jobDetail.JobDataMap.Returns(jobDataMap);

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey(JobKeys.ProviderNotificationEmail));

        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.JobDetail.Returns(jobDetail);
        jobContext.Trigger.Returns(trigger);

        var providerDataService = Substitute.For<IProviderDataService>();

        var job = new ProviderNotificationEmailJobBuilder()
            .Build(providerDataService);

        await job.Execute(jobContext);

        await providerDataService
            .Received(1)
            .SendProviderNotifications(frequency);
    }

    [Fact]
    public async Task Execute_Job_Does_Not_Call_Expected_Services_When_Notification_Frequency_Not_Supplied()
    {
        var jobDataMap = new JobDataMap();
        var jobDetail = Substitute.For<IJobDetail>();
        jobDetail.JobDataMap.Returns(jobDataMap);

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey(JobKeys.ProviderNotificationEmail));

        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.JobDetail.Returns(jobDetail);
        jobContext.Trigger.Returns(trigger);

        var logger = Substitute.For<ILogger<ProviderNotificationEmailJob>>();

        var providerDataService = Substitute.For<IProviderDataService>();

        var job = new ProviderNotificationEmailJobBuilder()
            .Build(providerDataService,
                logger);

        await job.Execute(jobContext);

        logger.ReceivedCalls().Count().Should().Be(1);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && (LogLevel)args[0] == LogLevel.Error);

        logger.ReceivedCalls()
            .Select(call => call.GetArguments())
            .Should()
            .Contain(args =>
                args[0] is LogLevel && 
                (LogLevel)args[0] == LogLevel.Error &&
                args[3] != null && args[3] is ArgumentException);

        await providerDataService
            .DidNotReceive()
            .SendProviderNotifications(Arg.Any<NotificationFrequency>());
    }
}
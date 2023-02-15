using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Jobs;

public class EmployerInterestCleanupJobTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerInterestCleanupJob)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(EmployerInterestCleanupJob)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task Execute_Job_Calls_Expected_Services()
    {
        const int count = 10;
        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .RemoveExpiredEmployerInterest()
            .Returns(count);

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey(JobKeys.EmployerInterestCleanup));
        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.Trigger.Returns(trigger);

        var job = new EmployerInterestCleanupJobBuilder()
            .Build(employerInterestService);

        await job.Execute(jobContext);

        await employerInterestService.Received(1).NotifyExpiringEmployerInterest();
        await employerInterestService.Received(1).RemoveExpiredEmployerInterest();
    }
}
using System.Threading.Tasks;
using NSubstitute;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Jobs;

public class InitializationJobTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(InitializationJob)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(InitializationJob)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task Execute_Job_Calls_Expected_Services_When_No_Qualifications_Or_Providers()
    {
        var courseDirectoryService = Substitute.For<ICourseDirectoryService>();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.HasProviders().Returns(false);
        providerDataService.HasQualifications().Returns(false);

        var townDataService = Substitute.For<ITownDataService>();
        townDataService.HasTowns().Returns(false);

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey("Test"));
        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.Trigger.Returns(trigger);

        var job = new InitializationJobBuilder()
            .Build(courseDirectoryService,
                   providerDataService,
                   townDataService);

        await job.Execute(jobContext);

        await courseDirectoryService.Received(1).ImportQualifications();
        await courseDirectoryService.Received(1).ImportProviders();
        await townDataService.Received(1).ImportTowns();
    }

    [Fact]
    public async Task Execute_Job_Calls_Expected_Services_When_Has_Qualifications_And_Providers()
    {
        var courseDirectoryService = Substitute.For<ICourseDirectoryService>();

        var providerDataService = Substitute.For<IProviderDataService>();
        providerDataService.HasProviders().Returns(true);
        providerDataService.HasQualifications().Returns(true);

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey("Test"));
        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.Trigger.Returns(trigger);

        var job = new InitializationJobBuilder()
            .Build(courseDirectoryService, providerDataService);

        await job.Execute(jobContext);

        await courseDirectoryService.DidNotReceive().ImportQualifications();
        await courseDirectoryService.DidNotReceive().ImportProviders();
    }
}
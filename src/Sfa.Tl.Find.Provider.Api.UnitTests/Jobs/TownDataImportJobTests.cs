using NSubstitute;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Jobs;

public class TownDataImportJobTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(TownDataImportJob)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(TownDataImportJob)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task Execute_Job_Calls_Expected_Services()
    {
        var townDataService = Substitute.For<ITownDataService>();

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey("Test"));
        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.Trigger.Returns(trigger);

        var job = new TownDataImportJobBuilder()
            .Build(townDataService);

        await job.Execute(jobContext);

        await townDataService.Received(1).ImportTowns();
    }
}
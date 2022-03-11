using System.Threading.Tasks;
using NSubstitute;
using Quartz;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Jobs;

public class TownDataImportJobTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(TownDataImportJob)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
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
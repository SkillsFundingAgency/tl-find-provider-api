using Quartz;
using Sfa.Tl.Find.Provider.Api.Jobs;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Jobs;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Jobs;

public class CourseDataImportJobTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(CourseDataImportJob)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(CourseDataImportJob)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task Execute_Job_Calls_Expected_Services()
    {
        var courseDirectoryService = Substitute.For<ICourseDirectoryService>();

        var trigger = Substitute.For<ITrigger>();
        trigger.JobKey.Returns(new JobKey(JobKeys.CourseDataImport));
        var jobContext = Substitute.For<IJobExecutionContext>();
        jobContext.Trigger.Returns(trigger);

        var job = new CourseDataImportJobBuilder()
            .Build(courseDirectoryService);

        await job.Execute(jobContext);

        //await courseDirectoryService.Received(1).ImportQualifications();
        await courseDirectoryService.Received(1).ImportProviders();
    }
}
using Quartz;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class JobTriggersControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(JobTriggersController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(JobTriggersController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task TriggerCourseDirectoryImportJob_Runs_Job()
    {
        var scheduler = Substitute.For<IScheduler>();
        var schedulerFactory = Substitute.For<ISchedulerFactory>();
        schedulerFactory.GetScheduler()
            .Returns(Task.FromResult(scheduler));

        var controller = new JobTriggersControllerBuilder()
            .Build(schedulerFactory);

        await controller.TriggerCourseDirectoryImportJob();

        await scheduler
            .Received(1)
            .TriggerJob(Arg.Is<JobKey>( k => 
                k.Name == JobKeys.CourseDataImport));
    }

    [Fact]
    public async Task TriggerEmployerInterestCleanupJob_Runs_Job()
    {
        var scheduler = Substitute.For<IScheduler>();
        var schedulerFactory = Substitute.For<ISchedulerFactory>();
        schedulerFactory.GetScheduler()
            .Returns(Task.FromResult(scheduler));

        var controller = new JobTriggersControllerBuilder()
            .Build(schedulerFactory);

        await controller.TriggerEmployerInterestCleanupJob();

        await scheduler
            .Received(1)
            .TriggerJob(Arg.Is<JobKey>(k => 
                k.Name == JobKeys.EmployerInterestCleanup));
    }

    [Fact]
    public async Task TriggerProviderNotificationEmailImmediateJob_Runs_Job()
    {
        var scheduler = Substitute.For<IScheduler>();
        var schedulerFactory = Substitute.For<ISchedulerFactory>();
        schedulerFactory.GetScheduler()
            .Returns(Task.FromResult(scheduler));

        var controller = new JobTriggersControllerBuilder()
            .Build(schedulerFactory);

        await controller.TriggerProviderNotificationEmailImmediateJob();

        await scheduler
            .Received(1)
            .TriggerJob(Arg.Is<JobKey>(k =>
                k.Name == JobKeys.ProviderNotificationEmailImmediate));
    }

    [Fact]
    public async Task TriggerProviderNotificationEmailDailyJob_Runs_Job()
    {
        var scheduler = Substitute.For<IScheduler>();
        var schedulerFactory = Substitute.For<ISchedulerFactory>();
        schedulerFactory.GetScheduler()
            .Returns(Task.FromResult(scheduler));

        var controller = new JobTriggersControllerBuilder()
            .Build(schedulerFactory);

        await controller.TriggerProviderNotificationEmailDailyJob();

        await scheduler
            .Received(1)
            .TriggerJob(Arg.Is<JobKey>(k =>
                k.Name == JobKeys.ProviderNotificationEmailDaily));
    }

    [Fact]
    public async Task TriggerProviderNotificationEmailWeeklyJob_Runs_Job()
    {
        var scheduler = Substitute.For<IScheduler>();
        var schedulerFactory = Substitute.For<ISchedulerFactory>();
        schedulerFactory.GetScheduler()
            .Returns(Task.FromResult(scheduler));

        var controller = new JobTriggersControllerBuilder()
            .Build(schedulerFactory);

        await controller.TriggerProviderNotificationEmailWeeklyJob();

        await scheduler
            .Received(1)
            .TriggerJob(Arg.Is<JobKey>(k =>
                k.Name == JobKeys.ProviderNotificationEmailWeekly));
    }

    [Fact]
    public async Task TriggerStartupTasksJob_Runs_Job()
    {
        var scheduler = Substitute.For<IScheduler>();
        var schedulerFactory = Substitute.For<ISchedulerFactory>();
        schedulerFactory.GetScheduler()
            .Returns(Task.FromResult(scheduler));

        var controller = new JobTriggersControllerBuilder()
            .Build(schedulerFactory);

        await controller.TriggerStartupTasksJob();

        await scheduler
            .Received(1)
            .TriggerJob(Arg.Is<JobKey>(k => 
                k.Name == JobKeys.StartupTasks));
    }

    [Fact]
    public async Task TriggerImportTownDataJob_Runs_Job()
    {
        var scheduler = Substitute.For<IScheduler>();
        var schedulerFactory = Substitute.For<ISchedulerFactory>();
        schedulerFactory.GetScheduler()
            .Returns(Task.FromResult(scheduler));

        var controller = new JobTriggersControllerBuilder()
            .Build(schedulerFactory);

        await controller.TriggerImportTownDataJob();

        await scheduler
            .Received(1)
            .TriggerJob(Arg.Is<JobKey>(k =>
                k.Name == JobKeys.ImportTownData));
    }
}
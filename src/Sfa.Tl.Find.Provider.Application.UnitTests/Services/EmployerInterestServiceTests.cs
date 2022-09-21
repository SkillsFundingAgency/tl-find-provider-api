using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;

public class EmployerInterestServiceTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(EmployerInterestService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_BadParameters()
    {
        typeof(EmployerInterestService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task RemoveExpiredEmployerInterest_Does_Not_Call_Repository_For_Zero_RetentionDays()
    {
        var settings = new EmployerInterestSettings
        {
            RetentionDays = 0
        };

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2022-08-13"));

        //Expected date is Today - RetentionDays 
        //var expectedDate = DateTime.Parse("2022-08-01");

        //const int count = 9;

        var repository = Substitute.For<IEmployerInterestRepository>();
        repository.DeleteBefore(Arg.Any<DateTime>())
            .Returns(0);

        var service = new EmployerInterestServiceBuilder()
            .Build(dateTimeService,
                repository,
                settings);

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(0);

        await repository
            .DidNotReceive()
            .DeleteBefore(Arg.Any<DateTime>());
    }

    [Fact]
    public async Task RemoveExpiredEmployerInterest_Calls_Repository()
    {
        var settings = new EmployerInterestSettings
        {
            RetentionDays = 12
        };

        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService.Today.Returns(DateTime.Parse("2022-08-13"));

        //Expected date is Today - RetentionDays 
        var expectedDate = DateTime.Parse("2022-08-01");

        const int count = 9;

        var repository = Substitute.For<IEmployerInterestRepository>();
        repository.DeleteBefore(Arg.Any<DateTime>())
            .Returns(count);

        var service = new EmployerInterestServiceBuilder()
            .Build(
                dateTimeService, 
                repository,
                settings
                );

        var result = await service.RemoveExpiredEmployerInterest();

        result.Should().Be(count);

        await repository
            .Received(1)
            .DeleteBefore(Arg.Any<DateTime>());

        /*
    NSubstitute.Exceptions.ReceivedCallsException : Expected to receive exactly 1 call matching:
	DeleteBefore(d => (d == value(Sfa.Tl.Find.Provider.Application.UnitTests.Services.EmployerInterestServiceTests+<>c__DisplayClass2_0).expectedDate))
Actually received no matching calls.
Received 1 non-matching call (non-matching arguments indicated with '*' characters):
	DeleteBefore(*03/08/2022 00:00:00*)         */
        await repository
            .Received(1)
            .DeleteBefore(Arg.Is<DateTime>(d => d == expectedDate));
    }

    [Fact]
    public async Task FindEmployerInterest_Returns_Expected_List()
    {
        var employerInterests = new EmployerInterestBuilder()
            .BuildList()
            .ToList();

        var employerInterestRepository = Substitute.For<IEmployerInterestRepository>();
        employerInterestRepository.GetAll()
            .Returns(employerInterests);

        var service = new EmployerInterestServiceBuilder()
            .Build(employerInterestRepository: employerInterestRepository);

        var results = (await service.FindEmployerInterest()).ToList();
        results.Should().BeEquivalentTo(employerInterests);

        await employerInterestRepository
            .Received(1)
            .GetAll();
    }
}
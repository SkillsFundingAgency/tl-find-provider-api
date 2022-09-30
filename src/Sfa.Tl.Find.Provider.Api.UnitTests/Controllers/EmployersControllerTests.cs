using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class EmployersControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(EmployersController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(EmployersController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }
    
    [Fact]
    public async Task DeleteInterest_Returns_Ok_Result_For_Successful_Deletion()
    {
        var uniqueId = Guid.Parse("5AF374D2-1072-4E98-91CF-6AE765044DBA");
        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .DeleteEmployerInterest(uniqueId)
            .Returns(1);

        var controller = new EmployersControllerBuilder()
            .Build(employerInterestService);

        var result = await controller.DeleteInterest(uniqueId);

        result.Should().BeOfType(typeof(NoContentResult));
    }

    [Fact]
    public async Task DeleteInterest_Returns_NotFound_Result_For_NoItemsDeleted()
    {
        var uniqueId = Guid.Parse("5AF374D2-1072-4E98-91CF-6AE765044DBA");
        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .DeleteEmployerInterest(uniqueId)
            .Returns(0);

        var controller = new EmployersControllerBuilder()
            .Build(employerInterestService);

        var result = await controller.DeleteInterest(uniqueId);
        result.Should().BeOfType(typeof(NotFoundResult));
    }

}
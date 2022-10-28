using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
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
    public async Task CreateInterest_Returns_Ok_Result_For_Successful_Creation()
    {
        var uniqueId = Guid.Parse("6f3606b9-8323-49d5-b405-14bacb3a82e5");

        var employerInterest = new EmployerInterestBuilder().Build();

        var employerInterestService = Substitute.For<IEmployerInterestService>();
        employerInterestService
            .CreateEmployerInterest(Arg.Any<EmployerInterest>())
            .Returns(uniqueId);

        var controller = new EmployersControllerBuilder()
            .Build(employerInterestService);

        var result = await controller.CreateInterest(employerInterest);

        result.Should().BeOfType(typeof(OkObjectResult));
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        var jsonString = okResult!.Value?.ToString();
        var expectedJson = new { id = uniqueId };

        jsonString.Should().Be(expectedJson.ToString());
        expectedJson.Should().BeEquivalentTo(okResult.Value);
        okResult.Value.Should().BeEquivalentTo(expectedJson);
    }

    [Fact]
    public async Task DeleteInterest_Returns_NoContent_Result_For_Successful_Deletion()
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
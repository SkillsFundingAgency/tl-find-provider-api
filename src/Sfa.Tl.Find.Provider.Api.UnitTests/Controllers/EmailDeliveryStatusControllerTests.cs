using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute.ExceptionExtensions;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Controllers;

public class EmailDeliveryStatusControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(EmailDeliveryStatusController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(EmailDeliveryStatusController)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task EmailDeliveryStatusCallback_Returns_Expected_Result()
    {
        var emailDeliverReceipt = new EmailDeliveryReceiptBuilder()
            .WithDeliveryStatus(EmailDeliveryStatus.Delivered)
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();

        var emailDeliveryStatusService = Substitute.For<IEmailDeliveryStatusService>();
        emailDeliveryStatusService.HandleEmailDeliveryStatus(emailDeliverReceipt).Returns(1);

        var controller = new EmailDeliveryStatusControllerBuilder()
            .WithAuthorizationToken(emailSettings.DeliveryStatusToken)
            .Build(emailDeliveryStatusService,
                emailSettings);

        var result = await controller.EmailDeliveryStatusCallback(emailDeliverReceipt);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        okResult.Value.Should().BeEquivalentTo("1 record(s) updated.");
    }

    [Fact]
    public async Task EmailDeliveryStatusCallback_Returns_Error_For_Missing_Authorization_Header()
    {
        var emailDeliverReceipt = new EmailDeliveryReceiptBuilder()
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();

        var emailDeliveryStatusService = Substitute.For<IEmailDeliveryStatusService>();
        emailDeliveryStatusService.HandleEmailDeliveryStatus(emailDeliverReceipt).Returns(1);

        var controller = new EmailDeliveryStatusControllerBuilder()
            .Build(emailDeliveryStatusService,
                emailSettings);

        var result = await controller.EmailDeliveryStatusCallback(emailDeliverReceipt);

        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.Value.Should().BeEquivalentTo("Missing or malformed 'Authorization' header.");
    }

    [Fact]
    public async Task EmailDeliveryStatusCallback_Returns_Error_For_Incorrect_Authorization_Header()
    {
        var emailDeliverReceipt = new EmailDeliveryReceiptBuilder()
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();

        var emailDeliveryStatusService = Substitute.For<IEmailDeliveryStatusService>();
        emailDeliveryStatusService.HandleEmailDeliveryStatus(emailDeliverReceipt).Returns(1);

        var controller = new EmailDeliveryStatusControllerBuilder()
            .WithAuthorizationToken("72b561ed-a7f3-4c0c-82a9-aae800a51de7")
            .Build(emailDeliveryStatusService,
                emailSettings);

        var result = await controller.EmailDeliveryStatusCallback(emailDeliverReceipt);

        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.Value.Should().BeEquivalentTo("Missing or malformed 'Authorization' header.");
    }

    [Fact]
    public async Task EmailDeliveryStatusCallback_Returns_Error_Result_When_Exception_Thrown()
    {
        var emailDeliverReceipt = new EmailDeliveryReceiptBuilder()
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();

        var emailDeliveryStatusService = Substitute.For<IEmailDeliveryStatusService>();
        emailDeliveryStatusService
            .HandleEmailDeliveryStatus(emailDeliverReceipt)
            .Throws(new Exception());

        var controller = new EmailDeliveryStatusControllerBuilder()
            .WithAuthorizationToken(emailSettings.DeliveryStatusToken)
            .Build(emailDeliveryStatusService,
                emailSettings);

        var result = await controller.EmailDeliveryStatusCallback(emailDeliverReceipt);

        var statusCodeResult = result as StatusCodeResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
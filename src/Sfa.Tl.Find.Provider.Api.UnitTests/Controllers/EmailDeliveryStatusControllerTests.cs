using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
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
    public async Task EmailDeliveryStatusCallback_Returns_Expected_List()
    {
        //if (!(Request.Headers
        //          .TryGetValue("Authorization", out var token)
        //      && token.Equals($"Bearer {_emailSettings.DeliveryStatusToken}")))

        var emailDeliverReceipt = new EmailDeliveryReceiptBuilder()
            //.WithDeliveryStatus("delivered")
            .Build();

        var emailSettings = new SettingsBuilder().BuildEmailSettings();

        var emailDeliveryStatusService = Substitute.For<IEmailDeliveryStatusService>();
        emailDeliveryStatusService.HandleEmailDeliveryStatus(emailDeliverReceipt).Returns(1);

        var controller = new EmailDeliveryStatusControllerBuilder()
            .Build(emailDeliveryStatusService,
                emailSettings);

        //var headers = new Dictionary<string, StringValues> { { "Authorization", "Bearer 72b561ed-a7f3-4c0c-82a9-aae800a51de7" } };

        controller
            .ControllerContext
            .HttpContext
            .Request
            .Headers
            .Add("Authorization",
                $"Bearer {emailSettings.DeliveryStatusToken}");

        var result = await controller.EmailDeliveryStatusCallback(emailDeliverReceipt);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        //var innerResult = okResult.Value as int;
        okResult.Value.Should().BeEquivalentTo("1 record(s) updated.");
    }

    //TODO: Add more tests
}
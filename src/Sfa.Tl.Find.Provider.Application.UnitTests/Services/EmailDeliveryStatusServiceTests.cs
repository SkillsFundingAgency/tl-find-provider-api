using Sfa.Tl.Find.Provider.Application.Services;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Services;
public class EmailDeliveryStatusServiceTests
{
    [Fact]
    public void Constructor_Guards_Against_Null_Parameters()
    {
        typeof(EmailDeliveryStatusService)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Constructor_Guards_Against_Bad_Parameters()
    {
        typeof(EmailDeliveryStatusService)
            .ShouldNotAcceptNullOrBadConstructorArguments();
    }

    [Fact]
    public async Task EmailDeliveryStatusService_Sends_Email()
    {

        var emailDeliverReceipt = new EmailDeliveryReceiptBuilder()
            //.WithDeliveryStatus("delivered")
            .Build();

        var emailDeliveryStatusService = new EmailDeliveryStatusServiceBuilder()
            .Build();

        var result = await emailDeliveryStatusService.HandleEmailDeliveryStatus(
            emailDeliverReceipt);

        result.Should().Be(1);
    }
}

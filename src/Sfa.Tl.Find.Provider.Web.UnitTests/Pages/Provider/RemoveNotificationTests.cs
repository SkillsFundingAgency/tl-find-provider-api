using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public  class RemoveNotificationTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(RemoveNotificationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task RemoveNotificationModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var removeNotificationsModel = new RemoveNotificationModelBuilder()
            .Build(providerSettings: settings);

        await removeNotificationsModel.OnGet();

        removeNotificationsModel.NotificationEmail.Should().NotBeNull();
    }
}

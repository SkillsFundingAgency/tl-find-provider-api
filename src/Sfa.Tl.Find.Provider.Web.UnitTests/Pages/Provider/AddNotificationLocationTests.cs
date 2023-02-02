using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages.Provider;
public class AddNotificationLocationTests
{
    private const int ProviderNotificationId = 1;

    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(AddNotificationLocationModel)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task AddNotificationLocationModel_OnGet_Sets_ExpectedProperties()
    {
        var settings = new SettingsBuilder().BuildProviderSettings();

        var addNotificationLocationModel = new AddNotificationLocationModelBuilder()
            .Build(providerSettings: settings);

        await addNotificationLocationModel.OnGet(ProviderNotificationId);
    }
}

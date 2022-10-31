using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Filters;

public class HmacAuthorizationFilterBuilder
{
    public HmacAuthorizationFilter Build(
        ApiSettings apiSettings = null,
        ICacheService cacheService = null,
        ILogger<HmacAuthorizationFilter> logger = null)
    {
        var apiSettingOptions = Options.Create(
            apiSettings
            ?? new SettingsBuilder()
                .BuildApiSettings());

        cacheService ??= Substitute.For<ICacheService>();
        logger ??= Substitute.For<ILogger<HmacAuthorizationFilter>>();

        return new HmacAuthorizationFilter(apiSettingOptions, cacheService, logger);
    }
}
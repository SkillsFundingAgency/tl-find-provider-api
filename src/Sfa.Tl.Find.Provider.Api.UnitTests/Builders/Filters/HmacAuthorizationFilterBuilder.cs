using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Filters;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders.Filters;

public class HmacAuthorizationFilterBuilder
{
    public HmacAuthorizationFilter Build(
        ApiSettings apiSettings = null,
        IMemoryCache memoryCache = null,
        ILogger<HmacAuthorizationFilter> logger = null)
    {
        var apiSettingOptions = Options.Create(
            apiSettings
            ?? new SettingsBuilder()
                .BuildApiSettings());

        memoryCache ??= Substitute.For<IMemoryCache>();
        logger ??= Substitute.For<ILogger<HmacAuthorizationFilter>>();

        return new HmacAuthorizationFilter(apiSettingOptions, memoryCache, logger);
    }
}
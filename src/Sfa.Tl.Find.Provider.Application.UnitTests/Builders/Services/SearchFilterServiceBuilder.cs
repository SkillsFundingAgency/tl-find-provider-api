using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Builders.Services;

public class SearchFilterServiceBuilder
{
    public ISearchFilterService Build(
        ISearchFilterRepository searchFilterRepository = null,
        ILogger<SearchFilterService> logger = null)
    {
        searchFilterRepository ??= Substitute.For<ISearchFilterRepository>();
        logger ??= Substitute.For<ILogger<SearchFilterService>>();

        return new SearchFilterService(
            searchFilterRepository,
            logger);
    }
}
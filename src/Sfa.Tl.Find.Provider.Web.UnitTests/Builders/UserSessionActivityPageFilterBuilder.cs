using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Filters;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class UserSessionActivityPageFilterBuilder
{
    public UserSessionActivityPageFilter Build(
        IMemoryCache? cache = null,
        IDateTimeService? dateTimeService = null)
    {
        cache ??= Substitute.For<IMemoryCache>();
        dateTimeService ??= Substitute.For<IDateTimeService>();

        var controller = new UserSessionActivityPageFilter(
            cache,
            dateTimeService);

        return controller;
    }
}

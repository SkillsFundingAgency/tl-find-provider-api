using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Filters;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class UserSessionActivityPageFilterBuilder
{
    public UserSessionActivityPageFilter Build(
        ICacheService? cacheService = null,
        IDateTimeService? dateTimeService = null)
    {
        cacheService ??= Substitute.For<ICacheService>();
        dateTimeService ??= Substitute.For<IDateTimeService>();

        var controller = new UserSessionActivityPageFilter(
            cacheService,
            dateTimeService);

        return controller;
    }
}

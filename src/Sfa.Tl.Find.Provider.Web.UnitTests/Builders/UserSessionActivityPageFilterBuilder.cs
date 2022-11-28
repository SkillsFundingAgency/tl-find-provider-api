using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Web.Filters;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class UserSessionActivityPageFilterBuilder
{
    public UserSessionActivityPageFilter Build(
        ICacheService? cacheService = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        cacheService ??= Substitute.For<ICacheService>();
        dateTimeProvider ??= Substitute.For<IDateTimeProvider>();

        var controller = new UserSessionActivityPageFilter(
            cacheService,
            dateTimeProvider);

        return controller;
    }
}

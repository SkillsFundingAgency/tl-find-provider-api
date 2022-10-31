using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Filters;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Filters;
public class UserSessionActivityPageFilterTests

{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(UserSessionActivityPageFilter)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public async Task OnPageHandlerExecution_Calls_Cache()
    {
        var timeNowUtc = new DateTime(2022, 11, 01, 10, 11, 12, 0, DateTimeKind.Utc);

        var cache = Substitute.For<IMemoryCache>();
        var dateTimeService = Substitute.For<IDateTimeService>();
        dateTimeService
            .UtcNow
            .Returns(timeNowUtc);

        var pageContext = new PageContextBuilder()
            .Build(true);

        var context = new PageHandlerExecutingContext(
            pageContext,
            Array.Empty<IFilterMetadata>(),
            new HandlerMethodDescriptor(),
            new Dictionary<string, object?>(),
            new object());

        var wasNextCalled = false;

        var pageHandlerExecutedContext = new PageHandlerExecutedContext(
            pageContext,
            Array.Empty<IFilterMetadata>(),
            new HandlerMethodDescriptor(),
            new object());

        PageHandlerExecutionDelegate next = () =>
        {
            wasNextCalled = true;
            return Task.FromResult(pageHandlerExecutedContext); 

        };

        var filter = new UserSessionActivityPageFilterBuilder()
            .Build(cache, dateTimeService);

        await filter.OnPageHandlerExecutionAsync(context,
            next);

        //cache
        //    .Received(1)
        //    .Set(Arg.Any<object>(),
        //        Arg.Any<object>()); //timeNowUtc
        cache
            .Received(1)
            .CreateEntry(Arg.Any<string>());
        cache
            .Received(1)
            .CreateEntry(Arg.Is<string>(k => k.StartsWith("USER")));

        wasNextCalled.Should().BeTrue();
    }
}

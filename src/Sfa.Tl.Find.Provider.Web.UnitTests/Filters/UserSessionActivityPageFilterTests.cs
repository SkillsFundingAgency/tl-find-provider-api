using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
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

        var cacheService = Substitute.For<ICacheService>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .UtcNow
            .Returns(timeNowUtc);

        var pageContext = new PageContextBuilder()
            .Build();

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

        Task<PageHandlerExecutedContext> Next()
        {
            wasNextCalled = true;
            return Task.FromResult(pageHandlerExecutedContext);
        }

        var filter = new UserSessionActivityPageFilterBuilder()
            .Build(cacheService, dateTimeProvider);

        await filter.OnPageHandlerExecutionAsync(context, Next);

        await cacheService
            .Received(1)
            .Set(Arg.Is<string>(k => k.StartsWith("USER")),
                timeNowUtc);

        wasNextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task OnPageHandlerExecution_Does_Not_Call_Cache_For_Unauthenticated_User()
    {
        var timeNowUtc = new DateTime(2022, 11, 01, 10, 11, 12, 0, DateTimeKind.Utc);

        var cacheService = Substitute.For<ICacheService>();
        var dateTimeProvider = Substitute.For<IDateTimeProvider>();
        dateTimeProvider
            .UtcNow
            .Returns(timeNowUtc);

        var pageContext = new PageContextBuilder()
            .Build(false);

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

        Task<PageHandlerExecutedContext> Next()
        {
            wasNextCalled = true;
            return Task.FromResult(pageHandlerExecutedContext);
        }

        var filter = new UserSessionActivityPageFilterBuilder()
            .Build(cacheService, dateTimeProvider);

        await filter.OnPageHandlerExecutionAsync(context, Next);

        await cacheService
            .DidNotReceive()
            .Set(Arg.Any<string>(),
                Arg.Any<object>());

        wasNextCalled.Should().BeTrue();
    }
}

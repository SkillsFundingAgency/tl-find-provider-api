using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Filters;

public class UserSessionActivityPageFilter : IAsyncPageFilter
{
    private readonly IMemoryCache _cache;
    private readonly IDateTimeService _dateTimeService;

    public UserSessionActivityPageFilter(
        IMemoryCache cache,
        IDateTimeService dateTimeService)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
        PageHandlerExecutionDelegate next)
    {
        if (context.HttpContext.User.Identity is { IsAuthenticated: true })
        {
            var cacheKey = context.HttpContext.User.GetUserSessionCacheKey();
            _cache.Set(cacheKey, _dateTimeService.UtcNow);
        }

        await next.Invoke();
    }
}
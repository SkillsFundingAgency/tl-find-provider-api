using Microsoft.AspNetCore.Mvc.Filters;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;

namespace Sfa.Tl.Find.Provider.Web.Filters;

public class UserSessionActivityPageFilter : IAsyncPageFilter
{
    private readonly ICacheService _cacheService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UserSessionActivityPageFilter(
        ICacheService cacheService,
        IDateTimeProvider dateTimeProvider)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
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
            _cacheService.Set(cacheKey, _dateTimeProvider.UtcNow);
        }

        await next.Invoke();
    }
}
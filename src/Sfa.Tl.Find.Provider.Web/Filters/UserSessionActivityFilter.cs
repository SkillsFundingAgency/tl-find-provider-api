using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Sfa.Tl.Find.Provider.Web.Extensions;

namespace Sfa.Tl.Find.Provider.Web.Filters;

public class SessionPageActivityFilter : IAsyncPageFilter //IPageFilter 
{
    private readonly IMemoryCache _cache;

    public SessionPageActivityFilter(
        IMemoryCache cache)
    {
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
        PageHandlerExecutionDelegate next)
    {
        if (context.HttpContext.User?.Identity is {IsAuthenticated: true})
        {
            var cacheKey = context.HttpContext.User.GetUserSessionCacheKey();
            _cache.Set(cacheKey, DateTime.UtcNow);
        }

        await next.Invoke();

        //public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        //{
        //}

        //public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        //{
        //}

        //public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        //{
        //}
    }
}
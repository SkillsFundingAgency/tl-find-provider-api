using System.Reflection;

namespace Sfa.Tl.Find.Provider.Api.Web.Services;

public record SearchTerms(string? Term, int Page, int PageSize)
{
    public static ValueTask<SearchTerms?> BindAsync(HttpContext httpContext, ParameterInfo parameter)
    {
        int.TryParse(httpContext.Request.Query["page"], out var page);
        int.TryParse(httpContext.Request.Query["page-size"], out var pageSize);

        return ValueTask.FromResult<SearchTerms?>(
            new SearchTerms(
                httpContext.Request.Query["term"],
                page == 0 ? 1 : page,
                pageSize == 0 ? 10 : pageSize
            )
        );
    }
}
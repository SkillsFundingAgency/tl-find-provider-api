using System.Text;

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class TracingExtensions
{
    public static void LogCookies(this ILogger logger, HttpContext? httpContext, string method)
    {
        try
        {
            if (httpContext is null) return;

            var requestCookies = httpContext.Request?.Cookies;
            var responseCookies = httpContext.Response?.Cookies;
            var sb = new StringBuilder();
            if (requestCookies != null)
            {
                sb.AppendLine($"{method} - request cookies:");
                foreach (var c in requestCookies)
                {
                    sb.AppendLine($"  {c.Key} - {c.Value?.Length ?? 0}");
                }
            }

            logger.LogDebug(sb.ToString());
        }
        catch
        {
            //Safely ignore any exceptions
        }
    }
}

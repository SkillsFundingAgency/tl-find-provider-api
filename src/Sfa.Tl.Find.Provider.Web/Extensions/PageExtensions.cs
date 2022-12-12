using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using System.Security.Claims;
using System.Text;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;


namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class PageExtensions
{
    private const string ServiceName = "Connect with employers interested in T Levels";

    public static string GetServiceName() => ServiceName;

    public static string GenerateTitle(
        string? title,
        bool isValid = true)
    {
        var formattedTitle = new StringBuilder();
        if (!isValid) formattedTitle.Append("Error: ");
        if (string.IsNullOrEmpty(title))
        {
            return formattedTitle.Append($"{ServiceName}").ToString();
        }

        if (title != ServiceName) formattedTitle.Append($"{title} | ");
        formattedTitle.Append($"{ServiceName}");

        return formattedTitle.ToString();
    }

    public static string FormatTitleWithAdministratorTag(string title, ClaimsPrincipal user)
    {
        var ukPrnClaim = user?.GetClaim(CustomClaimTypes.UkPrn);
        var userTypeTag = (ukPrnClaim is null
                           || (long.TryParse(ukPrnClaim, out var ukPrn) && ukPrn == 0))
                          && user is not null 
                          && user.IsInRole(CustomRoles.Administrator)
            ? " - Administrator"
            : null;

        return $"{title}{userTypeTag}";
    }
}

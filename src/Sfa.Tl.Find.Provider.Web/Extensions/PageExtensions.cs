using System.Text;

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class PageExtensions
{
    private const string ServiceName = "Connect with employers interested in T Levels";
    private const string GovUk = "GOV.UK";

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
}

using System.Text;

namespace Sfa.Tl.Find.Provider.Web.Extensions;

public static class PageExtensions
{
    private const string ServiceName = "T Levels Provider Data Service Home";
    private const string GovUk = "GOV.UK";

    public static string GetServiceName() => ServiceName;

    public static string GenerateTitle(
        string? title,
        bool isValid = true)
    {
        var ignoreTitle = title == ServiceName;
        const string titleSuffix = $"{ServiceName} - {GovUk}";

        var formattedTitle = new StringBuilder();
        if (!isValid) formattedTitle.Append("Error: ");
        if (string.IsNullOrEmpty(title))
        {
            return formattedTitle.Append(titleSuffix).ToString();
        }

        if (!ignoreTitle) formattedTitle.Append($"{title} - ");
        formattedTitle.Append(titleSuffix);

        return formattedTitle.ToString();
    }
}

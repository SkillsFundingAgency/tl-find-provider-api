using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

public static class PostcodeLocationExtensions
{
    public static string GetUriFormattedPostcode(this PostcodeLocation postcodeLocation)
    {
        return postcodeLocation.Postcode is not null ? 
            postcodeLocation.Postcode.Replace(" ", "%20")
            : string.Empty;
    }
}
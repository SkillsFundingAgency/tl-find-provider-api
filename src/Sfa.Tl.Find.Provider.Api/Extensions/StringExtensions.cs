using System;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class StringExtensions
    {
        public static string FormatPostcodeForUri(this string postcode)
        {
            return Uri.EscapeUriString(postcode.Trim().ToUpper());
        }
    }
}

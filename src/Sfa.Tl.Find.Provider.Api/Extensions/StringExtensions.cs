using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class StringExtensions
    {
        public static string FormatPostcodeForUri(this string postcode)
        {
            return Uri.EscapeUriString(postcode.Trim().ToUpper());
        }

        public static string ParseTLevelDefinitionName(this string fullName, int maxLength = -1)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "";

            var parts = fullName.Split('-');
            var name = Regex.Replace(parts[^1],
                    "^T Level in ", "", RegexOptions.IgnoreCase)
                .ToTitleCase();

            return name is not null && maxLength > 0 && name.Length > maxLength
                ? name[..maxLength].Trim()
                : name;
        }

        public static string ToTitleCase(this string value)
        {
            if (value == null)
                return null;

            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var result = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLowerInvariant());

            var tokens = result.Split(new[] { ' ', '\t', '\r', '\n' },
                    StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            var artsAndPreps = new List<string>
            {
                "a", "an", "and", "any", "at", "for", "from", "in", "into",
                "of", "on", "or", "some", "the", "to"
            };

            result = tokens[0];
            tokens.RemoveAt(0);

            result += tokens.Aggregate(string.Empty, (prev, input)
                => prev +
                   (artsAndPreps.Contains(input.ToLower())
                       ? " " + input.ToLower() // Return the prep/art lowercase
                       : " " + input));        // Otherwise return the valid word

            return result.Trim();
        }

    }
}

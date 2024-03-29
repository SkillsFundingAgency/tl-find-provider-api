﻿using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class StringExtensions
{
    // adapted from http://stackoverflow.com/a/164994/1882637
    private const string PostcodeRegex =
            @"(GIR 0AA)|((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX‌​]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY]))))\s?[0-9][A-Z-[C‌​IKMOV]]{2})(\w)*$";

    private const string PartialPostcodeRegex = @"((([A-Z-[QVX]][0-9][0-9]?)|(([A-Z-[QVX]][A-Z-[IJZ]][0-9][0-9]?)|(([A-Z-[QVX]][0-9][A-HJKSTUW])|([A-Z-[QVX]][A-Z-[IJZ]][0-9][ABEHMNPRVWXY])))))$";

    public static bool IsPostcode(this string postcode)
    {
        return CheckPostcode(postcode, PostcodeRegex);
    }

    public static bool IsPartialPostcode(this string postcode)
    {
        return CheckPostcode(postcode, PartialPostcodeRegex);
    }

    public static bool IsFullOrPartialPostcode(this string postcode)
    {
        return postcode.IsPostcode() || postcode.IsPartialPostcode();
    }

    private static bool CheckPostcode(string postcode, string regex)
    {
        if (string.IsNullOrWhiteSpace(postcode))
            return false;

        var formattedPostcode = postcode.Trim().ToUpperInvariant();

        return Regex.IsMatch(formattedPostcode, regex);
    }

    public static bool DoesNotMatch(this string input, params string[] patterns)
    {
        return !patterns.Any(p => Regex.IsMatch(input, p));
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

    public static string ToSearchableString(this string value)
    {
        if (value == null)
            return null;

        //Remove special characters and spaces, and replace & with and
        const string knownSpecialCharacters = @"(\s+|,|\.|'|\-|!|\(|\)|/)";
        return Regex.Replace(
                Regex.Replace(value, knownSpecialCharacters, ""),
                @"(&)", "and")
                .ToLower();
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

        //Fix S after apostrophe, if it is before space or at end of string
        result = Regex.Replace(result, @"(['’])S(\s|$)", "$1s$2");

        return result.Trim();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReplaceBreaksWithNewlines(this string input)
    {
        return input is null
            ? null
            : Regex.Replace(input, @"(<br ?>)|(<br ?/>)|(\[br ?/\])", "\n");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReplaceMultipleLineBreaks(this string input, string replacement = "\n")
    {
        return input is null
            ? null
            : Regex.Replace(input,
                "(\n|\r|\r\n){2,}", replacement);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ReplaceRedactedHttpStrings(this string input)
    {
        return input is null
            ? null
            : Regex.Replace(input, @"(http[s]?)___", "$1://");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToTrimmedOrNullString(this string input)
    {
        return input is null || input.Length == 0 || string.IsNullOrWhiteSpace(input)
            ? null
            : input.Trim();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Truncate(this string input, int length)
    {
        return input is null || input.Length <= length
            ? input
            : input[..length];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string TruncateWithEllipsis(this string input, int length)
    {
        return input is null || input.Length <= length
            ? input
            : input[..length] + "...";
    }
}
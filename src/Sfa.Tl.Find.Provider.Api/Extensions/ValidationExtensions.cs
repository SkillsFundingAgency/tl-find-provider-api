using System.Text.RegularExpressions;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class ValidationExtensions
{
    public static bool TryValidate(this string searchTerm, out string errorMessage)
    {
        errorMessage = null;
        var regex = new Regex(@"^[a-zA-Z][0-9a-zA-Z\s]*$");

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            errorMessage = "The search term is required.";
        }
        else if (!regex.IsMatch(searchTerm))
        {
            errorMessage = "The postcode field must start with a letter and contain only letters, numbers, and an optional space.";
        }
        else if (searchTerm.Length < 2)
        {
            errorMessage = "The search term must be at least 2 characters.";
        }
        else if (searchTerm.IsFullOrPartialPostcode())
        {
            errorMessage = searchTerm.Length switch
            {
                < 2 => "The postcode must be at least 2 characters.",
                > 8 => "The postcode must be no more than 8 characters.",
                _ => errorMessage
            };
        }

        return errorMessage is null;
    }
    
    public static bool TryValidate(
        this (double? latitude, double? longitude) latLong,
        out string errorMessage)
    {
        errorMessage = null;

        if (!latLong.latitude.HasValue || !latLong.longitude.HasValue)
        {
            errorMessage = "Both latitude and longitude required if postcode is not provided.";
        }

        return errorMessage is null;
    }

    public static bool TryValidate(
        this string searchTerm,
        double? latitude,
        double? longitude,
        out string errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(searchTerm) && !(latitude.HasValue && longitude.HasValue))
        {
            errorMessage = "Either search term or both lat/long required.";
            return false;
        }

        if (!string.IsNullOrWhiteSpace(searchTerm) && (latitude.HasValue || longitude.HasValue))
        {
            errorMessage = "Either search term or lat/long required, but not both.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            if (!(latitude, longitude).TryValidate(out errorMessage))
                return false;
        }
        else
        {
            var lettersAndDigitsRegex = new Regex(@"^[a-zA-Z][0-9a-zA-Z\s]*$");
            if (!lettersAndDigitsRegex.IsMatch(searchTerm))
            {
                errorMessage =
                    "The search term must start with a letter and contain only letters, numbers, and spaces.";
            }
            else
            {
                searchTerm.TryValidate(out errorMessage);
            }
        }

        return errorMessage is null;
    }
}
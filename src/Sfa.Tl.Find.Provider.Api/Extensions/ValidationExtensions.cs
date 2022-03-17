using System.Text.RegularExpressions;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

public static class ValidationExtensions
{
    public static bool TryValidate(this string postcode, out string errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(postcode))
        {
            errorMessage = "The postcode field is required.";
        }
        else
        {
            errorMessage = postcode.Length switch
            {
                < 2 => "The postcode field must be at least 2 characters.",
                > 8 => "The postcode field must be no more than 8 characters.",
                _ => errorMessage
            };

            var regex = new Regex(@"^[a-zA-Z][0-9a-zA-Z\s]*$");
            if (!regex.IsMatch(postcode))
                errorMessage = "The postcode field must start with a letter and contain only letters, numbers, and an optional space.";
        }

        return errorMessage is null;
    }

    public static bool TryValidate(
        this string postcode,
        double? latitude,
        double? longitude,
        out string errorMessage)
    {
        errorMessage = null;

        if (string.IsNullOrWhiteSpace(postcode) && !(latitude.HasValue && longitude.HasValue))
        {
            errorMessage = "Either postcode or both lat/long required.";
        }
        else if (!string.IsNullOrWhiteSpace(postcode) && (latitude.HasValue || longitude.HasValue))
        {
            errorMessage = "Either postcode or lat/long required, but not both.";
        }
        else if (string.IsNullOrWhiteSpace(postcode))
        {
            (latitude, longitude).TryValidate(out errorMessage);
        }
        else
        {
            postcode.TryValidate(out errorMessage);
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
}
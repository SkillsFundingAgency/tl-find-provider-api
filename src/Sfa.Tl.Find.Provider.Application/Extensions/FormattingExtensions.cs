using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class FormattingExtensions
{
    public static string FormatDistance(this double? distance)
    {
        return distance.HasValue ? distance.Value.FormatDistance() : string.Empty;
    }

    public static string FormatDistance(this double distance)
    {
        var roundedDistance = (int)Math.Round(distance);
        var formattedDistance = $"{roundedDistance:#0}";
        formattedDistance += formattedDistance == "1" ? " mile" : " miles";

        return formattedDistance;
    }

    public static string FormatPostcodeForUri(this string postcode)
    {
        return Uri.EscapeDataString(postcode.Trim().ToUpper());
    }

    public static string FormatTownName(this Town town)
    {
        if (!string.IsNullOrWhiteSpace(town.County))
            return $"{town.Name}, {town.County}";
        else if (!string.IsNullOrWhiteSpace(town.LocalAuthority))
            return $"{town.Name}, {town.LocalAuthority}";
        return $"{town.Name}";
    }

    public static string GetContactPreferenceDisplayName(
        this ContactPreference? contactPreference,
        string defaultDisplayName = "Unknown")
    {
        if (contactPreference is null) return string.Empty;
        
        var displayAttribute = contactPreference.Value.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name)
            ? displayAttribute.Name
            : defaultDisplayName;
    }
}
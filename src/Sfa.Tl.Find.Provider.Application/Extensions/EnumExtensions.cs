using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class EnumExtensions
{
    public static TAttribute GetCustomAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        return name != null ? type.GetField(name!)!
            .GetCustomAttribute<TAttribute>()
                : null;
    }
    
    public static string GetEnumDisplayName<T>(
        this T? value,
        string defaultDisplayName = "Unknown") where T : struct, Enum
    {
        if (value is null) return string.Empty;

        var displayAttribute = value.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name)
            ? displayAttribute.Name
            : defaultDisplayName;
    }

    public static string GetEnumDisplayName<T>(
        this T value) where T : struct, Enum
    {
        var displayAttribute = value.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute != null && !string.IsNullOrWhiteSpace(displayAttribute.Name)
            ? displayAttribute.Name
            : value.ToString();
    }
}
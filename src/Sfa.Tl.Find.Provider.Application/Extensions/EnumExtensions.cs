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
}
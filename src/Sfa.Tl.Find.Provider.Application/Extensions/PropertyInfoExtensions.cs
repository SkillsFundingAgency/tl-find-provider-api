using System.Reflection;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class PropertyInfoExtensions
{
    public static bool HasAttribute<TAttribute>(this PropertyInfo property)
        where TAttribute : Attribute
    {
        return property.GetCustomAttribute<TAttribute>() != null;
    }
}

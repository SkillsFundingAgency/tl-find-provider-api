using Dapper;
using System.Reflection;

namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;
public static class DynamicParameterValidationExtensions
{
    public static IList<object> GetDynamicTemplates(this DynamicParameters dynamicParameters)
    {
        var fieldInfo = dynamicParameters.GetType()
            .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
            .SingleOrDefault(p => p.Name == "templates");

        var templates = fieldInfo?.GetValue(dynamicParameters) as IList<object>;
        return templates;
    }

    public static int GetDynamicTemplatesCount(this IList<object> dynamicTemplates)
    {
        var item = dynamicTemplates.First();
        var pi = item.GetType().GetProperties();
        return pi.Length;
    }

    public static void HasCount(this IList<object> dynamicTemplates, int expectedCount)
    {
        var item = dynamicTemplates.First();
        var pi = item.GetType().GetProperties();
        pi.Length.Should().Be(expectedCount);
    }

    public static void ContainsNameAndValue<T>(
        this IList<object> dynamicTemplates,
        string name,
        T expectedValue)
    {
        var item = dynamicTemplates.First();
        var pi = item.GetType().GetProperties();
        pi.Length.Should().BeGreaterThan(0);

        var property = pi.SingleOrDefault(p => p.Name == name);
        property.Should().NotBeNull();
        property!.GetValue(item).Should().Be(expectedValue);
    }

    public static T GetParameter<T>(
        this IList<object> dynamicTemplates,
        string name)
    {
        var item = dynamicTemplates.First();
        var pi = item.GetType().GetProperties();
        pi.Length.Should().BeGreaterThan(0);

        var property = pi.SingleOrDefault(p => p.Name == name);
        property.Should().NotBeNull();
        return (T)property!.GetValue(item);
    }
}


namespace Sfa.Tl.Find.Provider.Tests.Common.Extensions;

public static class AssertionExtensions
{
    public static bool ListIsEquivalentTo(this IEnumerable<int> baseList, IEnumerable<int> compareList)
    {
        baseList.Should().BeEquivalentTo(compareList);

        //If assertion above passes, then return true
        return true;
    }
}
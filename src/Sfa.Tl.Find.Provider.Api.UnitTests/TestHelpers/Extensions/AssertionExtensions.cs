using System.Collections.Generic;
using FluentAssertions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;

public static class AssertionExtensions
{
    public static bool ListIsEquivalentTo(this IEnumerable<int> baseList, IEnumerable<int> compareList)
    {
        baseList.Should().BeEquivalentTo(compareList);

        //If assertion above passes, then return true
        return true;
    }
}
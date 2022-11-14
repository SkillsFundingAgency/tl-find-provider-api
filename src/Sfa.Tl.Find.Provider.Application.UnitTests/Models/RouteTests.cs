using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Application.UnitTests.Models;

public class RouteTests
{
    [Fact]
    public void Route_NumberOfQualifications_Returns_Expected_Count_For_Null_Qualifications()
    {
        var route = new Route();
        route.NumberOfQualifications.Should().Be(0);
    }

    [Fact]
    public void Route_NumberOfQualifications_Returns_Expected_Count()
    {
        var routes = new RouteBuilder().BuildList().ToList();

        routes.Last().NumberOfQualifications.Should().Be(routes.Last().Qualifications.Count);
    }

    [Fact]
    public void Route_NumberOfQualificationsOffered_Returns_Expected_Count_For_Null_Qualifications()
    {
        var route = new Route();
        route.NumberOfQualificationsOffered.Should().Be(0);
    }

    [Fact]
    public void Route_NumberOfQualificationsOffered_Returns_Expected_Count()
    {
        var routes = new RouteBuilder().BuildList().ToList();
        
        //Take a route
        var route = routes.Last();
        var expectedCount = route.Qualifications.Sum(q => q.NumberOfQualificationsOffered);
        route.NumberOfQualificationsOffered.Should().Be(expectedCount);
    }
}

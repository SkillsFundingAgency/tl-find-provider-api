using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Authorization;
using Sfa.Tl.Find.Provider.Web.Controllers;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Controllers;
public class TimeoutControllerTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(TimeoutController)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void TimeoutController_SignOutComplete_Returns_RedirectToPageResult()
    {
        var controller = new TimeoutControllerBuilder()
            .Build();

        var result = controller.TimeoutConfirmation();

        var redirectResult = result as RedirectToPageResult;
        redirectResult.Should().NotBeNull();
        redirectResult!.PageName.Should().Be(AuthenticationExtensions.UnauthenticatedUserStartPage);
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using Sfa.Tl.Find.Provider.Tests.Common.Extensions;
using Sfa.Tl.Find.Provider.Web.Pages.Error;
using Sfa.Tl.Find.Provider.Web.UnitTests.Builders;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Pages;
public class Error403PageTests
{
    [Fact]
    public void Constructor_Guards_Against_NullParameters()
    {
        typeof(Error403Model)
            .ShouldNotAcceptNullConstructorArguments();
    }

    [Fact]
    public void Error403Model_OnGet_Populates_Page_Properties()
    {
        var indexModel = new Error403ModelBuilder().Build();

        indexModel.OnGet();
    }

    [Fact]
    public void Error403Model_OnGetContactSupportClick_Returns_Expected_Result()
    {
        var indexModel = new Error403ModelBuilder().Build();

        var result = indexModel.OnGetContactSupportClick();

        result.Should().NotBeNull();
        result.Should().BeOfType(typeof(RedirectToPageResult));

        var redirectResult = result as RedirectToPageResult;
        redirectResult!.PageName.Should().Be("/Error/403");
    }
}
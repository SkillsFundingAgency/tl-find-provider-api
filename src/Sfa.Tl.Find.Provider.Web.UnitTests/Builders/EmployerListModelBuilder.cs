﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Sfa.Tl.Find.Provider.Web.Pages;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class EmployerListModelBuilder
{
    public EmployerListModel Build(
        IEmployerInterestService? employerInterestService = null,
        IPostcodeLookupService? postcodeLookupService = null,
        IProviderDataService? providerDataService = null,
        ISessionService? sessionService = null,
        EmployerInterestSettings? employerInterestSettings = null,
        ILogger<EmployerListModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true)
    {
        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated);

        employerInterestService ??= Substitute.For<IEmployerInterestService>();
        postcodeLookupService ??= Substitute.For<IPostcodeLookupService>();
        providerDataService ??= Substitute.For<IProviderDataService>();
        sessionService ??= Substitute.For<ISessionService>();
        logger ??= Substitute.For<ILogger<EmployerListModel>>();

        var employerInterestOptions = Options.Create(
            employerInterestSettings
            ?? new SettingsBuilder()
                .BuildEmployerInterestSettings());

        var pageModel = new EmployerListModel(
            employerInterestService,
            postcodeLookupService,
            providerDataService,
            sessionService,
            employerInterestOptions,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}

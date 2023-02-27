using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Authorization;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using System.Security.Claims;
using Sfa.Tl.Find.Provider.Web.Pages.Provider;

namespace Sfa.Tl.Find.Provider.Web.UnitTests.Builders;
public class SearchFilterDetailsModelBuilder
{
    public SearchFilterDetailsModel Build(
        IProviderDataService? providerDataService = null,
        ISearchFilterService? searchFilterService = null,
        ProviderSettings? providerSettings = null,
        ILogger<SearchFilterDetailsModel>? logger = null,
        PageContext? pageContext = null,
        bool userIsAuthenticated = true,
        bool isAdministrator = false)
    {
        var claims = userIsAuthenticated && isAdministrator
            ? new List<Claim>
            {
                new(ClaimTypes.Role, CustomRoles.Administrator)
            }
            : null;

        pageContext ??= new PageContextBuilder()
            .Build(userIsAuthenticated, claims);

        providerDataService ??= Substitute.For<IProviderDataService>();
        searchFilterService ??= Substitute.For<ISearchFilterService>();
        logger ??= Substitute.For<ILogger<SearchFilterDetailsModel>>();

        var providerOptions = Options.Create(
            providerSettings
            ?? new SettingsBuilder()
                .BuildProviderSettings());

        var pageModel = new SearchFilterDetailsModel(
            providerDataService,
            searchFilterService,
            providerOptions,
            logger)
        {
            PageContext = pageContext
        };

        return pageModel;
    }
}

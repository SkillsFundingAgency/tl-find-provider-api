using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Application.Models.Configuration;
using Sfa.Tl.Find.Provider.Application.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

//https://gunnarpeipman.com/aspnet-core-integration-test-startup/
public class FakeStartup
{
    private readonly SiteConfiguration _siteConfiguration;

    public FakeStartup()
    {
        _siteConfiguration = new SettingsBuilder().BuildConfigurationOptions();
    }

    public void Configure(IApplicationBuilder app)
    {
        app
            .UseRouting()
            .UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .Configure<ApiSettings>(x =>
            {
                x.AppId = _siteConfiguration.ApiSettings.AppId;
                x.ApiKey = _siteConfiguration.ApiSettings.ApiKey;
            })
            .Configure<CourseDirectoryApiSettings>(x =>
            {
                x.BaseUri = _siteConfiguration.CourseDirectoryApiSettings.BaseUri;
                x.ApiKey = _siteConfiguration.CourseDirectoryApiSettings.ApiKey;
            })
            .Configure<PostcodeApiSettings>(x =>
            {
                x.BaseUri = _siteConfiguration.PostcodeApiSettings.BaseUri;
            })
            .Configure<ConnectionStringSettings>(x =>
            {
                x.SqlConnectionString = _siteConfiguration.SqlConnectionString;
            });

        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(
                Constants.DefaultApiMajorVersion, 
                Constants.DefaultApiMinorVersion);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });

        services
            .AddControllers()
            //https://stackoverflow.com/questions/58679912/how-to-use-a-controller-in-another-assembly-in-asp-net-core-3-0
            .AddApplicationPart(typeof(ProvidersController).Assembly);

        services.Configure<RouteOptions>(options =>
        {
            options.AppendTrailingSlash = true;
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        services.AddMemoryCache();

        services
            .AddScoped(_ => Substitute.For<IDbContextWrapper>())
            .AddScoped<IDateTimeService, DateTimeService>()
            .AddTransient(_ =>
            {
                var providerDataService = Substitute.For<IProviderDataService>();
                providerDataService.FindProviders(
                        Arg.Any<string>(), 
                        Arg.Any<List<int>>(), 
                        Arg.Any<List<int>>(),
                        Arg.Any<int>())
                    .Returns(x => new ProviderSearchResponse
                    {
                        SearchTerm = (string)x[0],
                        SearchResults = new List<ProviderSearchResult>()
                    });
                providerDataService.FindProviders(
                        Arg.Any<double>(),
                        Arg.Any<double>(),
                        Arg.Any<List<int>>(),
                        Arg.Any<List<int>>(),
                        Arg.Any<int>())
                    .Returns(new ProviderSearchResponse
                    {
                        SearchTerm = "CV1 2WT",
                        SearchResults = new List<ProviderSearchResult>()
                    });

                providerDataService.GetAllProviders()
                    .Returns(new ProviderDetailResponse
                    {
                        Providers = new List<ProviderDetail>()
                    });

                return providerDataService;
            })
            .AddTransient(_ =>
            {
                var townDataService = Substitute.For<ITownDataService>();
                townDataService.Search(
                        Arg.Any<string>())
                    .Returns(_ => 
                        new TownBuilder()
                        .BuildList());

                return townDataService;
            })

            .AddTransient(_ => Substitute.For<IProviderRepository>())
            .AddTransient(_ => Substitute.For<IQualificationRepository>())
            .AddTransient(_ => Substitute.For<ITownRepository>());
    }
}
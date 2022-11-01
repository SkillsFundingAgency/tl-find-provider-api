using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Sfa.Tl.Find.Provider.Api.Controllers;
using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;
using Sfa.Tl.Find.Provider.Infrastructure.Caching;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Extensions;
using Sfa.Tl.Find.Provider.Infrastructure.Interfaces;
using Sfa.Tl.Find.Provider.Infrastructure.Services;
using Sfa.Tl.Find.Provider.Tests.Common.Builders.Models;
using Constants = Sfa.Tl.Find.Provider.Application.Models.Constants;
using RouteBuilder = Sfa.Tl.Find.Provider.Tests.Common.Builders.Models.RouteBuilder;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.IntegrationTests;

//https://gunnarpeipman.com/aspnet-core-integration-test-startup/
public class FakeStartup
{
    private readonly SiteConfiguration _siteConfiguration;
    private readonly TestConfigurationSettings _testConfigurationSettings;

    public FakeStartup()
    {
        _siteConfiguration = new SettingsBuilder().BuildConfigurationOptions();
        _testConfigurationSettings = new TestConfigurationSettings();
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
                x.ConfigureApiSettings(_siteConfiguration);
            })
            .Configure<CourseDirectoryApiSettings>(x =>
            {
                x.ConfigureCourseDirectoryApiSettings(_siteConfiguration);
            })
            .Configure<PostcodeApiSettings>(x =>
            {
                x.ConfigurePostcodeApiSettings(_siteConfiguration);
            })
            .Configure<ConnectionStringSettings>(x =>
            {
                x.ConfigureConnectionStringSettings(_siteConfiguration);
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
            .AddSingleton(_ => _testConfigurationSettings)
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
                providerDataService.GetCsv()
                    .Returns(new byte[]
                    {
                        084, 101, 115, 116
                    });
                providerDataService.GetQualifications()
                    .Returns(new QualificationBuilder()
                        .BuildList());
                providerDataService.GetRoutes()
                    .Returns(new RouteBuilder()
                        .BuildList());

                return providerDataService;
            })
            .AddTransient(_ =>
            {
                var employerInterestService = Substitute.For<IEmployerInterestService>();
                employerInterestService.CreateEmployerInterest(
                    Arg.Any<EmployerInterest>())
                        .Returns(_testConfigurationSettings.EmployerInterestUniqueId);
                employerInterestService.DeleteEmployerInterest(
                        Arg.Any<Guid>())
                    .Returns(1);

                return employerInterestService;
            })
            .AddTransient(_ =>
            {
                var townDataService = Substitute.For<ITownDataService>();
                townDataService.Search(
                        Arg.Any<string>())
                    .Returns(new TownBuilder()
                        .BuildList());

                return townDataService;
            })
            .AddTransient(_ =>
            {
                var postcodeLookupService = Substitute.For<IPostcodeLookupService>();
                postcodeLookupService.GetPostcode(
                        Arg.Any<string>())
                    .Returns(new GeoLocation
                    {
                        Location = "CV1 2WT",
                        Latitude = 52.400997,
                        Longitude = -1.508122
                    });
                postcodeLookupService.IsValid(
                        Arg.Any<string>())
                    .Returns(true);


                return postcodeLookupService;
            })
            .AddTransient(_ => Substitute.For<IProviderRepository>())
            .AddTransient(_ => Substitute.For<IQualificationRepository>())
            .AddTransient(_ => Substitute.For<ITownRepository>())
            .AddTransient<ICacheService, MemoryCacheService>();
    }
}
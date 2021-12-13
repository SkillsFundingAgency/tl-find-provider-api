﻿using System.Net.Http;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private const string CorsTestPolicyName = "TestPolicy";

        [Fact]
        public void AddApiVersioningPolicy_Should_AddService()
        {
            var services = new ServiceCollection();

            services.AddApiVersioningPolicy();

            services.Should().Contain(t => t.ServiceType.Name == "IApiVersionRoutePolicy");
        }

        [Fact]
        public void AddConfigurationOptions_Should_Add_Expected_Options()
        {
            var services = new ServiceCollection();

            var siteConfiguration = new SettingsBuilder().BuildConfigurationOptions();

            services.AddConfigurationOptions(null, siteConfiguration);

            services.Should().NotBeEmpty();

            var serviceProvider = services
                .BuildServiceProvider();

            var apiOptions = serviceProvider.GetRequiredService<IOptions<ApiSettings>>();
            apiOptions.Value.Should().BeEquivalentTo(siteConfiguration.ApiSettings);

            var courseDirectoryApiOptions = serviceProvider.GetRequiredService<IOptions<CourseDirectoryApiSettings>>();
            courseDirectoryApiOptions.Value.Should().BeEquivalentTo(siteConfiguration.CourseDirectoryApiSettings);

            var postcodeApiOptions = serviceProvider.GetRequiredService<IOptions<PostcodeApiSettings>>();
            postcodeApiOptions.Value.Should().BeEquivalentTo(siteConfiguration.PostcodeApiSettings);

            var connectionStringOptions = serviceProvider.GetRequiredService<IOptions<ConnectionStringSettings>>();
            //connectionStringOptions.Value.Should().BeEquivalentTo(siteConfiguration.ConnectionStringSettings);
            connectionStringOptions.Value.Should().NotBeNull();
            connectionStringOptions.Value.SqlConnectionString.Should().BeEquivalentTo(siteConfiguration.SqlConnectionString);
        }

        [Fact]
        public void AddCorsPolicy_Should_Not_Add_Policy_If_Allowed_Origins_Is_Null()
        {
            var services = new ServiceCollection();

            services.AddCorsPolicy(CorsTestPolicyName, null);
            services.Should().BeEmpty();
        }

        [Fact]
        public void AddCorsPolicy_Should_Not_Add_Policy_If_Allowed_Origins_Is_Empty()
        {
            var services = new ServiceCollection();

            services.AddCorsPolicy(CorsTestPolicyName, "");
            services.Should().BeEmpty();
        }

        [Fact]
        public void AddCorsPolicy_Should_Add_Policy_With_Allowed_Origins()
        {
            const string allowedOrigins = "*";

            var services = new ServiceCollection();

            services.AddCorsPolicy(CorsTestPolicyName, allowedOrigins);

            services.Should().Contain(t => t.ServiceType.Name == "ICorsService");
        }

        [Fact]
        public void AddHttpClients_Should_Add_IHttpClientFactory()
        {
            var services = new ServiceCollection();

            var siteConfiguration = new SettingsBuilder().BuildConfigurationOptions();

            services.AddConfigurationOptions(null, siteConfiguration);
            services.AddHttpClients();

            services.Should().NotBeEmpty();

            var serviceProvider = services
                .BuildServiceProvider();

            var factory = serviceProvider.GetService<IHttpClientFactory>();
            factory.Should().NotBeNull();
        }

        [Fact]
        public void AddQuartzServices_With_Cron_Schedule_Should_AddService()
        {
            var services = new ServiceCollection();

            services.AddQuartzServices("0 0 9 ? * MON-FRI");

            services.Should().Contain(t =>
                t.ImplementationType != null &&
                t.ImplementationType.Name == "QuartzHostedService");
        }

        [Fact]
        public void AddQuartzServices_Without_Cron_Schedule_Should_AddService()
        {
            var services = new ServiceCollection();

            services.AddQuartzServices(null);

            services.Should().Contain(t =>
                t.ImplementationType != null &&
                t.ImplementationType.Name == "QuartzHostedService");
        }

        [Fact]
        public void AddRateLimitPolicy_Should_AddService()
        {
            var services = new ServiceCollection();

            services.AddRateLimitPolicy();

            services.Should().Contain(t => t.ServiceType.Name == "IClientPolicyStore");
            services.Should().Contain(t => t.ServiceType.Name == "IIpPolicyStore");
            services.Should().Contain(t => t.ServiceType.Name == "IRateLimitCounterStore");
            services.Should().Contain(t => t.ServiceType.Name == "IProcessingStrategy");
            services.Should().Contain(t => t.ServiceType.Name == "IRateLimitConfiguration");
        }

        [Fact]
        public void AddSwagger_Should_AddService()
        {
            var services = new ServiceCollection();

            services.AddSwagger(
                "v1",
                "T Levels Find a Provider Api",
                "v1",
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            services.Should().Contain(t => t.ServiceType.Name == "ISwaggerProvider");
        }
    }
}

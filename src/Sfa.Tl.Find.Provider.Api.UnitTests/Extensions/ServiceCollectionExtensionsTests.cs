using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private const string TestPolicyName = "TestPolicy";

        [Fact]
        public void AddCorsPolicy_Should_Not_Add_Policy_If_Allowed_Origins_Is_Null()
        {
            var services = new ServiceCollection();

            services.AddCorsPolicy(TestPolicyName, null);
            services.Should().BeEmpty();
        }

        [Fact]
        public void AddCorsPolicy_Should_Not_Add_Policy_If_Allowed_Origins_Is_Empty()
        {
            var services = new ServiceCollection();

            services.AddCorsPolicy(TestPolicyName, "");
            services.Should().BeEmpty();
        }

        [Fact]
        public void AddCorsPolicy_Should_Add_Policy_With_Allowed_Origins()
        {
            const string allowedOrigins = "*";

            var services = new ServiceCollection();

            services.AddCorsPolicy(TestPolicyName, allowedOrigins);

            services.Should().NotBeEmpty();
            services.Should().Contain(t => t.ServiceType.Name == "ICorsService");
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

            services.Should().NotBeEmpty();
            services.Should().Contain(t => t.ServiceType.Name == "ISwaggerProvider");
        }

        [Fact]
        public void AddHostedQuartzServices_With_Cron_Schedule_Should_AddService()
        {
            var services = new ServiceCollection();

            services.AddHostedQuartzServices("0 0 9 ? * MON-FRI");

            services.Should().Contain(t => 
                t.ImplementationType != null && 
                t.ImplementationType.Name == "QuartzHostedService");
        }

        [Fact]
        public void AddHostedQuartzServices_Without_Cron_Schedule_Should_AddService()
        {
            var services = new ServiceCollection();

            services.AddHostedQuartzServices(null);

            services.Should().Contain(t => 
                t.ImplementationType != null && 
                t.ImplementationType.Name == "QuartzHostedService");
        }
    }
}

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

            var servicesAfter = services.AddCorsPolicy(TestPolicyName, null);
            servicesAfter.Should().BeEmpty();
        }

        [Fact]
        public void AddCorsPolicy_Should_Not_Add_Policy_If_Allowed_Origins_Is_Empty()
        {
            var services = new ServiceCollection();

            var servicesAfter = services.AddCorsPolicy(TestPolicyName, "");
            servicesAfter.Should().BeEmpty();
        }
        
        [Fact]
        public void AddCorsPolicy_Should_Add_Policy_With_Allowed_Origins()
        {
            const string allowedOrigins = "*";

            var services = new ServiceCollection();

            var servicesAfter = services.AddCorsPolicy(TestPolicyName, allowedOrigins);
            servicesAfter.Should().NotBeEmpty();
           
            servicesAfter.Should().Contain(t => t.ServiceType.Name == "ICorsService");
        }

        [Fact]
        public void AddSwagger_Should_AddService()
        {
            var services = new ServiceCollection();

            var servicesAfter = services.AddSwagger(
                "v1",
                "T Levels Find a Provider Api",
                "v1",
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            servicesAfter.Should().NotBeEmpty();
            servicesAfter.Should().Contain(t => t.ServiceType.Name == "ISwaggerProvider");

        }
    }
}

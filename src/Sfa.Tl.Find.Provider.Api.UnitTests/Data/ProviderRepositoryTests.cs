﻿using System.Threading.Tasks;
using FluentAssertions;
using Sfa.Tl.Find.Provider.Api.Data;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestEHelpers.Extensions;
using Xunit;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Data
{
    public class ProviderRepositoryTests
    {
        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(ProviderRepository)
                .ShouldNotAcceptNullConstructorArguments();
        }
        
        [Fact]
        public async Task GetProviders_Returns_Expected_List()
        {
            var repository = new ProviderRepositoryBuilder().Build();

            var results = await repository.GetAllProviders();
            results.Should().NotBeNullOrEmpty();
        }
    }
}

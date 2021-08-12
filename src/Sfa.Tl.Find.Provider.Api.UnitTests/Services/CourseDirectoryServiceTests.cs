using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Services;
using Sfa.Tl.Find.Provider.Api.UnitTests.Builders;
using Sfa.Tl.Find.Provider.Api.UnitTests.TestHelpers.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Services
{
    public class CourseDirectoryServiceTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ITestOutputHelper _output;

        public CourseDirectoryServiceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Constructor_Guards_Against_NullParameters()
        {
            typeof(CourseDirectoryService)
                .ShouldNotAcceptNullConstructorArguments();
        }

        [Fact]
        public void Constructor_Guards_Against_BadParameters()
        {
            typeof(CourseDirectoryService)
                .ShouldNotAcceptNullOrBadConstructorArguments();
        }

        [Fact]
        public async Task ImportProviders_Returns_Expected_Result()
        {
            const string getTLevelCoursesFragment = "tlevels";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelCoursesFragment, jsonBuilder.BuildValidTLevelsResponse() }
            };

            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Save(Arg.Any<IEnumerable<Models.Provider>>())
                .Returns((20, 10, 5));

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses, providerRepository);

            var (saved, updated, deleted) = await service.ImportProviders();

            saved.Should().Be(20);
            updated.Should().Be(10);
            deleted.Should().Be(5);
        }

        [Fact]
        public async Task ImportProviders_Creates_Expected_Providers()
        {
            const string getTLevelCoursesFragment = "tlevels";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelCoursesFragment, jsonBuilder.BuildValidTLevelsResponse() }
            };

            IList<Models.Provider> receivedProviders = null;

            var providerRepository = Substitute.For<IProviderRepository>();
            providerRepository.Save(Arg.Do<IEnumerable<Models.Provider>>(
                    x => receivedProviders = x?.ToList()))
                .Returns((20, 0, 0));

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses, providerRepository);

            await service.ImportProviders();

            receivedProviders.Should().NotBeNull();
            receivedProviders.Count.Should().Be(1);

            var targetProvider = receivedProviders.SingleOrDefault(p => p.UkPrn == 10000055);
            targetProvider.Should().NotBeNull();

            targetProvider!.UkPrn.Should().Be(10000055);
            targetProvider.Name.Should().BeEquivalentTo("ABINGDON AND WITNEY COLLEGE");

            targetProvider.AddressLine1.Should().Be("Wootton Road");
            targetProvider.AddressLine2.Should().BeNull();
            targetProvider.Town.Should().Be("Abingdon");
            targetProvider.County.Should().Be("Oxfordshire");
            targetProvider.Postcode.Should().Be("OX14 1GG");
            targetProvider.Email.Should().Be("test.user@abingdon-witney.ac.uk");
            targetProvider.Telephone.Should().Be("01234 555555");
            targetProvider.Website.Should().Be("http://www.abingdon-witney.ac.uk");

            //VerifyProvider(targetProvider);
        }

        [Fact]
        public async Task ImportQualifications_Returns_Expected_Result()
        {
            const string getTLevelDefinitionsUriFragment = "tleveldefinitions";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelDefinitionsUriFragment, jsonBuilder.BuildValidTLevelDefinitionsResponse() }
            };

            var qualificationRepository = Substitute.For<IQualificationRepository>();
            qualificationRepository.Save(Arg.Any<IEnumerable<Qualification>>())
                .Returns((10, 5, 2));

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses, qualificationRepository: qualificationRepository);

            var (saved, updated, deleted) = await service.ImportQualifications();

            saved.Should().Be(10);
            updated.Should().Be(5);
            deleted.Should().Be(2);
        }

        [Fact]
        public async Task ImportQualifications_Creates_Expected_Qualifications()
        {
            const string getTLevelDefinitionsUriFragment = "tleveldefinitions";

            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { getTLevelDefinitionsUriFragment, jsonBuilder.BuildValidTLevelDefinitionsResponse() }
            };

            IList<Qualification> receivedQualifications = null;

            var qualificationRepository = Substitute.For<IQualificationRepository>();
            qualificationRepository
                .Save(Arg.Do<IEnumerable<Qualification>>(
                    x => receivedQualifications = x?.ToList()))
                .Returns((16, 0, 0));

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses, qualificationRepository: qualificationRepository);

            await service.ImportQualifications();

            receivedQualifications.Should().NotBeNull();
            receivedQualifications.Count.Should().Be(16);

            receivedQualifications.Should().Contain(q => q.Id == 36 && q.Name == "Design, Surveying and Planning for Construction");
            receivedQualifications.Should().Contain(q => q.Id == 37 && q.Name == "Digital Production, Design and Development");
            receivedQualifications.Should().Contain(q => q.Id == 38 && q.Name == "Education and Childcare");
            receivedQualifications.Should().Contain(q => q.Id == 39 && q.Name == "Digital Business Services");
            receivedQualifications.Should().Contain(q => q.Id == 40 && q.Name == "Digital Support Services");
            receivedQualifications.Should().Contain(q => q.Id == 41 && q.Name == "Health");
            receivedQualifications.Should().Contain(q => q.Id == 42 && q.Name == "Healthcare Science");
            receivedQualifications.Should().Contain(q => q.Id == 43 && q.Name == "Science");
            receivedQualifications.Should().Contain(q => q.Id == 44 && q.Name == "Onsite Construction");
            receivedQualifications.Should().Contain(q => q.Id == 45 && q.Name == "Building Services Engineering for Construction");
            receivedQualifications.Should().Contain(q => q.Id == 46 && q.Name == "Finance");
            receivedQualifications.Should().Contain(q => q.Id == 47 && q.Name == "Accounting");
            receivedQualifications.Should().Contain(q => q.Id == 48 && q.Name == "Design and Development for Engineering and Manufacturing");
            receivedQualifications.Should().Contain(q => q.Id == 49 && q.Name == "Maintenance, Installation and Repair for Engineering and Manufacturing");
            receivedQualifications.Should().Contain(q => q.Id == 50 && q.Name == "Engineering, Manufacturing, Processing and Control");
            receivedQualifications.Should().Contain(q => q.Id == 51 && q.Name == "Management and Administration");
        }
    }
}

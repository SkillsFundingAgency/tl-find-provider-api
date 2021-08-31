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
        public async Task ImportProviders_Creates_Expected_Providers()
        {
            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { CourseDirectoryService.CourseDetailEndpoint, jsonBuilder.BuildValidTLevelsResponse() }
            };

            IList<Models.Provider> receivedProviders = null;

            var providerRepository = Substitute.For<IProviderRepository>();
            await providerRepository.Save(Arg.Do<IList<Models.Provider>>(
                    x => receivedProviders = x?.ToList()));

            var service = new CourseDirectoryServiceBuilder()
                .Build(responses, providerRepository);

            await service.ImportProviders();

            receivedProviders.Should().NotBeNull();
            receivedProviders.Count.Should().Be(1);

            var targetProvider = receivedProviders.SingleOrDefault(p => p.UkPrn == 10000055);
            targetProvider.Should().NotBeNull();

            targetProvider!.UkPrn.Should().Be(10000055);
            targetProvider.Name.Should().BeEquivalentTo("ABINGDON AND WITNEY COLLEGE");

            ValidateProvider(targetProvider, 
                10000055,
                "ABINGDON AND WITNEY COLLEGE",
                "Wootton Road",
                null,
                "Abingdon",
                "Oxfordshire",
                "OX14 1GG",
                "test.user@abingdon-witney.ac.uk",
                "01234 555555",
                "http://www.abingdon-witney.ac.uk",
                2);

            var abingdonLocation = targetProvider.Locations.Single(l => l.Postcode == "OX14 1GG");
            ValidateLocation(abingdonLocation,
                "ABINGDON CAMPUS",
                "OX14 1GG",
                "Wootton Road",
                "Northcourt",
                "Abingdon",
                "Oxfordshire",
                "enquiry@abingdon-witney.ac.uk",
                "01234 555555",
                "http://www.abingdon-witney.ac.uk",
                51.680637,
                -1.286943,
                1);
            
            ValidateDeliveryYear(abingdonLocation.DeliveryYears.First(), 2021, new[] { 37, 38, 41 });
            var witneyLocation = targetProvider.Locations.Single(l => l.Postcode == "OX28 6NE");
            
            ValidateLocation(witneyLocation,
                "WITNEY CAMPUS",
                "OX28 6NE",
                "Holloway Road",
                "",
                "Witney",
                "Oxfordshire",
                "enquiry@abingdon-witney.ac.uk",
                "01993 777777",
                "http://www.abingdon-witney.ac.uk",
                51.785444,
                -1.487948,
                1);

            ValidateDeliveryYear(witneyLocation.DeliveryYears.First(), 2021, new[] { 37, 38, 41 });
        }
        
        [Fact]
        public async Task ImportQualifications_Creates_Expected_Qualifications()
        {
            var jsonBuilder = new CourseDirectoryJsonBuilder();

            var responses = new Dictionary<string, string>
            {
                { CourseDirectoryService.QualificationsEndpoint, jsonBuilder.BuildValidTLevelDefinitionsResponse() }
            };

            IList<Qualification> receivedQualifications = null;

            var qualificationRepository = Substitute.For<IQualificationRepository>();
            await qualificationRepository
                .Save(Arg.Do<IEnumerable<Qualification>>(
                    x => receivedQualifications = x?.ToList()));

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

        private static void ValidateProvider(Models.Provider provider, 
            long ukPrn,
            string name,
            string addressLine1,
            string addressLine2,
            string town,
            string county,
            string postcode,
            string email,
            string telephone,
            string website,
            int locationCount = 0)
        {
            provider.UkPrn.Should().Be(ukPrn);
            provider.Name.Should().Be(name);
            provider.AddressLine1.Should().Be(addressLine1);
            provider.AddressLine2.Should().Be(addressLine2);
            provider.Town.Should().Be(town);
            provider.County.Should().Be(county);
            provider.Postcode.Should().Be(postcode);
            provider.Email.Should().Be(email);
            provider.Telephone.Should().Be(telephone);
            provider.Website.Should().Be(website);

            provider.Locations.Should().NotBeNull();
            provider.Locations.Should().HaveCount(locationCount);
        }

        private static void ValidateLocation(Location location, 
            string name,
            string postcode,
            string addressLine1,
            string addressLine2,
            string town,
            string county,
            string email,
            string telephone,
            string website,
            double latitude, 
            double longitude,
            int deliveryYearCount = 0)
        {
            location.Name.Should().Be(name);

            location.Postcode.Should().Be(postcode);
            location.AddressLine1.Should().Be(addressLine1);
            location.AddressLine2.Should().Be(addressLine2);
            location.Town.Should().Be(town);
            location.County.Should().Be(county);
            location.Email.Should().Be(email);
            location.Telephone.Should().Be(telephone);
            location.Website.Should().Be(website);

            location.Latitude.Should().Be(latitude);
            location.Longitude.Should().Be(longitude);

            location.DeliveryYears.Should().NotBeNull();
            location.DeliveryYears.Should().HaveCount(deliveryYearCount);
        }
        
        private static void ValidateDeliveryYear(DeliveryYear deliveryYear, short year, IReadOnlyCollection<int> qualificationIds)
        {
            deliveryYear.Year.Should().Be(year);

            deliveryYear.Qualifications.Should().NotBeNull();
            deliveryYear.Qualifications.Should().HaveCount(qualificationIds.Count);

            foreach (var qualificationId in qualificationIds)
            {
                deliveryYear.Qualifications.Should().Contain(q => q.Id == qualificationId);
            }
        }
    }
}

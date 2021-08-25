using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Services
{
    public class CourseDirectoryService : ICourseDirectoryService
    {
        private readonly HttpClient _httpClient;

        private readonly IProviderRepository _providerRepository;
        private readonly IQualificationRepository _qualificationRepository;
        private readonly ILogger<CourseDirectoryService> _logger;

        public const string CourseDetailEndpoint = "tlevels";
        public const string QualificationsEndpoint = "tleveldefinitions";

        public CourseDirectoryService(
            HttpClient httpClient,
            IProviderRepository providerRepository,
            IQualificationRepository qualificationRepository,
            ILogger<CourseDirectoryService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _providerRepository = providerRepository ?? throw new ArgumentNullException(nameof(providerRepository));
            _qualificationRepository = qualificationRepository ?? throw new ArgumentNullException(nameof(qualificationRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ImportProviders()
        {
            var responseMessage = await _httpClient.GetAsync(CourseDetailEndpoint);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Course Directory API call to '{CourseDetailEndpoint}' failed with " +
                                 $"{responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
            }

            responseMessage.EnsureSuccessStatusCode();

            var providers = await ReadTLevelProvidersFromResponse(responseMessage);

            await _providerRepository.Save(providers);

            _logger.LogInformation($"{nameof(CourseDirectoryService)} saved providers.");
        }

        public async Task ImportQualifications()
        {
            var responseMessage = await _httpClient.GetAsync(QualificationsEndpoint);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Course Directory API call to '{QualificationsEndpoint}' failed with " +
                                 $"{responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
            }

            responseMessage.EnsureSuccessStatusCode();

            var qualifications = await ReadTLevelQualificationsFromResponse(responseMessage);

            await _qualificationRepository.Save(qualifications);

            _logger.LogInformation($"{nameof(CourseDirectoryService)} saved qualifications.");
        }

        private async Task<IEnumerable<Models.Provider>> ReadTLevelProvidersFromResponse(
            HttpResponseMessage responseMessage)
        {
            var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

            var providers = new List<Models.Provider>();

            foreach (var courseElement in jsonDocument.RootElement
                .GetProperty("tLevels")
                .EnumerateArray()
                .Where(courseElement => courseElement.SafeGetString("offeringType") == "TLevel"))
            {
                var tLevelId = courseElement.SafeGetString("tLevelId");

                if (!courseElement.TryGetProperty("provider", out var providerElement))
                {
                    _logger.LogWarning($"Could not find provider property for course record with tLevelId {tLevelId}.");
                    continue;
                }

                if (!int.TryParse(providerElement.SafeGetString("ukprn"), out var ukPrn))
                {
                    _logger.LogWarning($"Could not find ukprn property for course record with tLevelId {tLevelId}");
                    continue;
                }

                var provider = providers.FirstOrDefault(p => p.UkPrn == ukPrn);

                if (provider == null)
                {
                    provider = new Models.Provider
                    {
                        UkPrn = ukPrn,
                        Name = providerElement.SafeGetString("providerName"),
                        AddressLine1 = providerElement.SafeGetString("addressLine1"),
                        AddressLine2 = providerElement.SafeGetString("addressLine2"),
                        Town = providerElement.SafeGetString("town"),
                        County = providerElement.SafeGetString("county"),
                        Postcode = providerElement.SafeGetString("postcode"),
                        Email = providerElement.SafeGetString("email"),
                        Telephone = providerElement.SafeGetString("telephone"),
                        Website = providerElement.SafeGetString("website"),
                        Locations = new List<Location>()
                    };
                    providers.Add(provider);
                }

                if (!courseElement.TryGetProperty("locations", out var locationsProperty))
                {
                    _logger.LogWarning(
                        $"Could not find locations property for course record with tLevelId {tLevelId}.");
                    continue;
                }

                var qualificationFrameworkCode = courseElement.TryGetProperty("qualification", out var qualificationProperty)
                    ? qualificationProperty.SafeGetInt32("frameworkCode")
                    : 0;
                if (qualificationFrameworkCode == 0)
                {
                    _logger.LogWarning($"Could not find qualification for course record with tLevelId {tLevelId}.");
                    continue;
                }

                if (!DateTime.TryParse(courseElement.SafeGetString("startDate"), out var startDate))
                {
                    _logger.LogWarning($"Could not read start date for course record with tLevelId {tLevelId}.");
                    continue;
                }

                foreach (var locationElement in locationsProperty.EnumerateArray())
                {
                    var postcode = locationElement.SafeGetString("postcode");

                    var location = provider.Locations.FirstOrDefault(l =>
                        l.Postcode == postcode);
                    if (location == null)
                    {
                        location = new Location
                        {
                            Postcode = postcode,
                            Name = locationElement.SafeGetString("venueName"),
                            AddressLine1 = locationElement.SafeGetString("addressLine1"),
                            AddressLine2 = locationElement.SafeGetString("addressLine2"),
                            Town = locationElement.SafeGetString("town"),
                            County = locationElement.SafeGetString("county"),
                            Email = locationElement.SafeGetString("email"),
                            Telephone = locationElement.SafeGetString("telephone"),
                            Website = locationElement.SafeGetString("website"),
                            Latitude = locationElement.SafeGetDouble("latitude"),
                            Longitude = locationElement.SafeGetDouble("longitude"),
                            DeliveryYears = new List<DeliveryYear>()
                        };

                        provider.Locations.Add(location);
                    }

                    var deliveryYear = location
                        .DeliveryYears
                        .FirstOrDefault(dy => dy.Year == startDate.Year);
                    if (deliveryYear == null)
                    {
                        deliveryYear = new DeliveryYear
                        {
                            Year = (short)startDate.Year,
                            Qualifications = new List<Qualification>()
                        };
                        location.DeliveryYears.Add(deliveryYear);
                    }

                    if (deliveryYear.Qualifications.All(q => q.Id != qualificationFrameworkCode))
                    {
                        deliveryYear.Qualifications.Add(new Qualification { Id = qualificationFrameworkCode });
                    }
                }
            }

            return providers;
        }

        private async Task<IEnumerable<Qualification>> ReadTLevelQualificationsFromResponse(HttpResponseMessage responseMessage)
        {
            var jsonDocument = await JsonDocument.ParseAsync(await responseMessage.Content.ReadAsStreamAsync());

            return jsonDocument.RootElement
                .GetProperty("tLevelDefinitions")
                .EnumerateArray()
                .Select(q => new Qualification
                {
                    Id = q.SafeGetInt32("frameworkCode"),
                    Name = q.SafeGetString("name").ParseTLevelDefinitionName()
                }).ToList();
        }
    }
}

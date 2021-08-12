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

        public async Task<(int Saved, int Updated, int Deleted)> ImportProviders()
        {
            var responseMessage = await _httpClient.GetAsync(CourseDetailEndpoint);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Course Directory API call to '{CourseDetailEndpoint}' failed with " +
                                 $"{responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
            }

            responseMessage.EnsureSuccessStatusCode();

            var providers = await ReadTLevelProvidersFromResponse(responseMessage);

            var results = await _providerRepository.Save(providers);

            _logger.LogInformation(
                $"Saved providers - inserted {results.Inserted}, updated {results.Updated}, deleted {results.Deleted}.");
            
            return results;
        }

        public async Task<(int Saved, int Updated, int Deleted)> ImportQualifications()
        {
            var responseMessage = await _httpClient.GetAsync(QualificationsEndpoint);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Course Directory API call to '{QualificationsEndpoint}' failed with " +
                                 $"{responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
            }

            responseMessage.EnsureSuccessStatusCode();

            var qualifications = await ReadTLevelQualificationsFromResponse(responseMessage);

            var results = await _qualificationRepository.Save(qualifications);

            _logger.LogInformation(
                $"Saved qualifications - inserted {results.Inserted}, updated {results.Updated}, deleted {results.Deleted}.");

            return results;
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

                if (!courseElement.TryGetProperty("provider", out var providerProperty))
                {
                    _logger.LogWarning($"Could not find provider property for course record with tLevelId {tLevelId}.");
                    continue;
                }

                if (!int.TryParse(providerProperty.SafeGetString("ukprn"), out var ukPrn))
                {
                    _logger.LogWarning($"Could not find ukprn property for course record with tLevelId {tLevelId}.");
                    continue;
                }

                //var providerName = providerProperty.SafeGetString("providerName");

                var provider = providers.FirstOrDefault(p => p.UkPrn == ukPrn);

                if (provider == null)
                {
                    provider = new Models.Provider
                    {
                        UkPrn = ukPrn,
                        Name = providerProperty.SafeGetString("providerName"),
                        AddressLine1 = providerProperty.SafeGetString("addressLine1"),
                        AddressLine2 = providerProperty.SafeGetString("addressLine2"),
                        Town = providerProperty.SafeGetString("town"),
                        County = providerProperty.SafeGetString("county"),
                        Postcode = providerProperty.SafeGetString("postcode"),
                        Email = providerProperty.SafeGetString("email"),
                        Telephone = providerProperty.SafeGetString("telephone"),
                        Website = providerProperty.SafeGetString("website"),
                        //Locations = new List<Location>()
                    };
                    providers.Add(provider);
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

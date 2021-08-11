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

        public async Task<(int Saved, int Updated, int Deleted)> ImportQualifications()
        {
            var responseMessage = await _httpClient.GetAsync(QualificationsEndpoint);

            if (responseMessage.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("Course Directory API call failed with " +
                                 $"{responseMessage.StatusCode} - {responseMessage.ReasonPhrase}");
            }

            responseMessage.EnsureSuccessStatusCode();

            var qualifications = await ReadTLevelQualificationsFromResponse(responseMessage);

            return await _qualificationRepository.Save(qualifications);
        }

        public async Task<(int Saved, int Updated, int Deleted)> ImportProviders()
        {
            return (0, 0, 0);
        }

        private static async Task<IEnumerable<Qualification>> ReadTLevelQualificationsFromResponse(HttpResponseMessage responseMessage)
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

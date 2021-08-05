using System;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

// ReSharper disable UnusedMember.Global

namespace Sfa.Tl.Find.Provider.Api.UnitTests.Builders
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class SettingsBuilder
    {
        private const string FindCourseApiKey = "0f608e5d437f4baabc04a0bc2dabbc1b";
        private const string FindCourseApiBaseAbsoluteUri = "https://test.com/findacourse/api";
        public static readonly Uri FindCourseApiBaseUri = new(FindCourseApiBaseAbsoluteUri);
        private const string PostcodeRetrieverUri = "https://test.api.postcodes.io/";

        internal CourseDirectoryApiSettings BuildCourseDirectoryApiSettings(
            string findCourseApiBaseUri = FindCourseApiBaseAbsoluteUri,
            string findCourseApiKey = FindCourseApiKey) => new()
            {
                BaseUri = findCourseApiBaseUri,
                ApiKey = findCourseApiKey
            };

        internal PostcodeApiSettings BuildPostcodeApiSettings(
            string postcodeRetrieverUri = PostcodeRetrieverUri) => new()
        {
            BaseUri = postcodeRetrieverUri
        };

        internal ConfigurationOptions BuildConfigurationOptions(
            PostcodeApiSettings postcodeApiSettings = null,
            string sqlConnectionString = "TestConnection") => new()
            {
                PostcodeApiSettings = postcodeApiSettings ?? BuildPostcodeApiSettings(),
                SqlConnectionString = sqlConnectionString
            };
    }
}

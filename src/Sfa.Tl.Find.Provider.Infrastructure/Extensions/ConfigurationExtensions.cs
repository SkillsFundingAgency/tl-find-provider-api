using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;

namespace Sfa.Tl.Find.Provider.Infrastructure.Extensions;

public static class ConfigurationExtensions
{
    public static SiteConfiguration LoadConfigurationOptions(this IConfiguration configuration)
    {
        try
        {
            var environment = configuration[Constants.EnvironmentNameConfigKey];
            var storageConnectionString = configuration[Constants.ConfigurationStorageConnectionStringConfigKey];
            var version = configuration[Constants.VersionConfigKey];
            var serviceName = configuration[Constants.ServiceNameConfigKey];

            var tableClient = GetTableClient(storageConnectionString, environment); 

            var tableEntity = tableClient
                .Query<TableEntity>(
                    filter: $"PartitionKey eq '{environment}' and RowKey eq '{serviceName}_{version}'");

            var data = tableEntity.FirstOrDefault()?["Data"]?.ToString();

            if (data == null)
            {
                throw new NullReferenceException("Configuration data was null.");
            }

            return JsonSerializer.Deserialize<SiteConfiguration>(data, 
                new JsonSerializerOptions
                {
                    ReadCommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });
        }
        catch (AggregateException aex)
        {
            if (configuration[Constants.EnvironmentNameConfigKey] is "LOCAL" 
                && aex.InnerException is TaskCanceledException)
            {
                //Workaround to allow front-end developers to load config from app settings
                return configuration.LoadConfigurationOptionsFromAppSettings();
            }

            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Configuration could not be loaded. Please check your configuration files or see the inner exception for details", ex);
        }
    }

    public static SiteConfiguration LoadConfigurationOptionsFromAppSettings(this IConfiguration configuration) =>
        new()
        {
            AllowedCorsOrigins = configuration[Constants.AllowedCorsOriginsConfigKey],
            ApiSettings = configuration.GetSection(Constants.ApiSettingsConfigKey).Get<ApiSettings>(),
            CourseDirectoryApiSettings = configuration.GetSection(Constants.CourseDirectoryApiSettingsConfigKey).Get<CourseDirectoryApiSettings>(),
            DfeSignInSettings = configuration.GetSection(Constants.DfeSignInSettingsConfigKey).Get<DfeSignInSettings>(),
            EmailSettings = configuration.GetSection(Constants.EmailSettingsConfigKey).Get<EmailSettings>(),
            EmployerInterestSettings = configuration.GetSection(Constants.EmployerInterestSettingsConfigKey).Get<EmployerInterestSettings>(),
            GoogleMapsApiSettings = configuration.GetSection(Constants.GoogleMapsApiSettingsConfigKey).Get<GoogleMapsApiSettings>(),
            PostcodeApiSettings = configuration.GetSection(Constants.PostcodeApiSettingsConfigKey).Get<PostcodeApiSettings>(),
            RedisCacheConnectionString = configuration[Constants.RedisCacheConnectionStringConfigKey],
            SearchSettings = configuration.GetSection(Constants.SearchSettingsConfigKey).Get<SearchSettings>(),
            SqlConnectionString = configuration[Constants.SqlConnectionStringConfigKey],
            BlobStorageConnectionString = configuration[Constants.BlobStorageConnectionStringConfigKey],
            CourseDirectoryImportSchedule = configuration[Constants.CourseDirectoryImportScheduleConfigKey],
            TownDataImportSchedule = configuration[Constants.TownDataImportScheduleConfigKey]
        };

    private static TableClient GetTableClient(string storageConnectionString, string environment) =>
        new(storageConnectionString, "Configuration",
            environment == "LOCAL"
                ? new TableClientOptions //Options to allow development running without azure emulator
                {
                    Retry =
                    {
                        NetworkTimeout = TimeSpan.FromMilliseconds(500),
                        MaxRetries = 1
                    }
                }
                : default);

    public static bool IsLocal(this IConfiguration configuration) => 
        configuration[Constants.EnvironmentNameConfigKey].StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase);
}
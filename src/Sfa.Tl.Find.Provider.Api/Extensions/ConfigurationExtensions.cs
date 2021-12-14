using System;
using System.Linq;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.Extensions;

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
                
            var tableClient = new TableClient(storageConnectionString, "Configuration");
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
        catch (Exception ex)
        {
            throw new InvalidOperationException("Configuration could not be loaded. Please check your configuration files or see the inner exception for details", ex);
        }
    }
}
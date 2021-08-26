using System;
using System.Text.Json;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
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

                var conn = CloudStorageAccount.Parse(storageConnectionString);
                var tableClient = conn.CreateCloudTableClient();
                var table = tableClient.GetTableReference("Configuration");

                var operation = TableOperation.Retrieve(environment, $"{serviceName}_{version}");
                var result = table.ExecuteAsync(operation).GetAwaiter().GetResult();

                var dynResult = result.Result as DynamicTableEntity;
                var data = dynResult?.Properties["Data"].StringValue;

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
}

using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.DataProtection;
using Sfa.Tl.Find.Provider.Infrastructure.Configuration;

namespace Sfa.Tl.Find.Provider.Web.Authorization;

public static class DataProtectionExtensions
{
    private const string ContainerName = "dataprotection";
    private const string BlobName = "keys";

    public static IServiceCollection AddWebDataProtection(
        this IServiceCollection services,
        SiteConfiguration configuration,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "keys")))
                .SetApplicationName("TLevelsConnect");
        }
        else if (!string.IsNullOrEmpty(configuration.BlobStorageConnectionString))
        {
            services.AddDataProtection()
                .PersistKeysToAzureBlobStorage(
                    GetDataProtectionBlobTokenUri(configuration));
        }

        return services;
    }

    private static Uri GetDataProtectionBlobTokenUri(SiteConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration.BlobStorageConnectionString);
        var blobContainerClient = blobServiceClient
            .GetBlobContainerClient(ContainerName);
        blobContainerClient.CreateIfNotExists();

        var blobClient = blobContainerClient
            .GetBlobClient(BlobName);
        
        return blobClient
            .GenerateSasUri(
                BlobSasPermissions.Read | 
                BlobSasPermissions.Write | 
                BlobSasPermissions.Create, 
                DateTime.UtcNow.AddYears(1));
    }
}
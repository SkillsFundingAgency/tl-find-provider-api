namespace Sfa.Tl.Find.Provider.Infrastructure.Configuration;

public class ConnectionStringSettings
{
    public string? BlobStorageConnectionString { get; set; }
 
    public string? RedisConnectionString { get; set; }
    
    public string? SqlConnectionString { get; set; }
}
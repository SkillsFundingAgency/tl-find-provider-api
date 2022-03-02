using Dapper;
using Microsoft.Data.SqlClient;

namespace Sfa.Tl.Find.Provider.Api.Web.Services;

public class LocationService : ILocationService
{
    private readonly string _connectionString;

    public LocationService(IConfiguration config)
    {
        _connectionString = config["LocationDatabaseConnectionString"];
    }

    public async Task<IEnumerable<LocationSearchResult>> Search(
        SearchTerms searchTerms,
        int maxResults = 50)
    {
        //TODO: If we use this, should be a repository and use the DbContextWrapper
        await using var connection = new SqlConnection(_connectionString);
        connection.Open();
        
        var results = await connection.QueryAsync<LocationSearchResult>(
            //connection,
            "SELECT TOP (@maxResults) " +
            "            [LocationName], " +
            "            [CountyName], " +
            "            [LocalAuthorityName]," +
            "            [Lat], " +
            "            [Long] " +
            "FROM dbo.[Location] " +
            "WHERE [LocationName] LIKE @query",
            new
            {
                maxResults,
                query = $"{searchTerms.Term}%"
            });

        return results;
    }
}
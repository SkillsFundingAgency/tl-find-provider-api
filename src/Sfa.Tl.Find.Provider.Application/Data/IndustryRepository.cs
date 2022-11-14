using Sfa.Tl.Find.Provider.Application.Interfaces;
using Sfa.Tl.Find.Provider.Application.Models;

namespace Sfa.Tl.Find.Provider.Application.Data;

public class IndustryRepository : IIndustryRepository
{
    private readonly IDbContextWrapper _dbContextWrapper;

    public IndustryRepository(
        IDbContextWrapper dbContextWrapper)
    {
        _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
    }

    public async Task<IEnumerable<Industry>> GetAll()
    {
        using var connection = _dbContextWrapper.CreateConnection();

        return await _dbContextWrapper.QueryAsync<Industry>(
            connection,
            "SELECT Id, Name " +
            "FROM dbo.Industry " +
            "WHERE IsDeleted = 0 " +
            "ORDER BY Name");
    }
}
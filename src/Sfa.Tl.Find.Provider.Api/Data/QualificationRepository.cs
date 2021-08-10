using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data
{
    public class QualificationRepository : IQualificationRepository
    {
        private readonly IDbContextWrapper _dbContextWrapper;

        public QualificationRepository(IDbContextWrapper dbContextWrapper)
        {
            _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        }

        public async Task<IEnumerable<Qualification>> GetAll()
        {
            using var connection = _dbContextWrapper.CreateConnection();

            return await _dbContextWrapper.QueryAsync<Qualification>(
                connection,
                "SELECT Id, Name " +
                "FROM dbo.Qualification " +
                "ORDER BY Name");
        }

        public async Task Save(IEnumerable<Qualification> qualifications)
        {
            using var connection = _dbContextWrapper.CreateConnection();
            await connection.ExecuteAsync("UpdateQualifications",
                new
                {
                    data = qualifications.AsTableValuedParameter("dbo.QualificationDataTableType")
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}

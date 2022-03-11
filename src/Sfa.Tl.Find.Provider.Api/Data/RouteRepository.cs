using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;

namespace Sfa.Tl.Find.Provider.Api.Data
{
    public class RouteRepository : IRouteRepository
    {
        private readonly IDbContextWrapper _dbContextWrapper;

        public RouteRepository(
            IDbContextWrapper dbContextWrapper)
        {
            _dbContextWrapper = dbContextWrapper ?? throw new ArgumentNullException(nameof(dbContextWrapper));
        }

        public async Task<IEnumerable<Route>> GetAll()
        {
            using var connection = _dbContextWrapper.CreateConnection();

            return await _dbContextWrapper.QueryAsync<Route>(
                connection,
                "SELECT r.Id, r.Name, COUNT(q.Id) AS NumberOfQualifications " +
                "FROM dbo.Route r " +
                "LEFT JOIN dbo.RouteQualification rq " +
                "ON rq.RouteId = r.Id " +
                "LEFT JOIN dbo.Qualification q " +
                "ON q.Id = rq.QualificationId " +
                "AND q.IsDeleted = 0 " +
                "WHERE r.IsDeleted = 0 " +
                "GROUP BY r.Id, r.Name " +
                "ORDER BY r.Name");
        }
    }
}
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces
{
    public interface IDbContextWrapper
    {
        IDbConnection CreateConnection();
        Task<IEnumerable<T>> QueryAsync<T>(IDbConnection connection, string sql);

        Task<int> ExecuteAsync(IDbConnection connection, string sql, 
            object param = null, 
            IDbTransaction transaction = null, 
            int? commandTimeout = null, 
            CommandType? commandType = null);
    }
}

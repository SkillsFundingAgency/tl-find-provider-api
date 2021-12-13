using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.Data
{
    public class DbContextWrapper : IDbContextWrapper
    {
        private readonly string _connectionString;
        private readonly ILogger<DbContextWrapper> _logger;

        public DbContextWrapper(
            //string connectionString,
            SiteConfiguration configuration,
            ILogger<DbContextWrapper> logger)
        {
            //_connectionString = connectionString
            //                    ?? throw new ArgumentNullException(nameof(connectionString));
            if(configuration is null) throw new ArgumentNullException(nameof(configuration));

            _connectionString = configuration.SqlConnectionString;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IDbConnection CreateConnection() => 
            new SqlConnection(_connectionString);

        public IDbTransaction BeginTransaction(IDbConnection connection) => 
            connection.BeginTransaction();

        public async Task<IEnumerable<T>> QueryAsync<T>(
            IDbConnection connection,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null) =>
            await connection.QueryAsyncWithRetry<T>(sql, param, transaction, commandTimeout, commandType);
        
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
            IDbConnection connection,
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null) =>
            await connection.QueryAsyncWithRetry<TFirst, TSecond, TThird, TReturn>(
                sql, map, param, transaction,
                splitOn, commandTimeout, commandType);

        public async Task<T> ExecuteScalarAsync<T>(
            IDbConnection connection, 
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null) =>
            await connection.ExecuteScalarAsync<T>(
                sql, param, transaction,
                commandTimeout, commandType);
    }
}

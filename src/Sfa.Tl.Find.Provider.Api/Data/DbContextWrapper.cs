using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Sfa.Tl.Find.Provider.Api.Interfaces;

namespace Sfa.Tl.Find.Provider.Api.Data
{
    public class DbContextWrapper : IDbContextWrapper
    {
        private readonly string _connectionString;

        public DbContextWrapper(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            IDbConnection connection,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
            IDbConnection connection,
            string sql,
            Func<TFirst, TSecond, TReturn> map, object param = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return await connection.QueryAsync<TFirst, TSecond, TReturn>(
                sql, map, param, transaction,
                splitOn: splitOn, commandTimeout: commandTimeout, commandType: commandType);
        }

        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
            IDbConnection connection,
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return await connection.QueryAsync<TFirst, TSecond, TThird, TReturn>(
                sql, map, param, transaction,
                splitOn: splitOn, commandTimeout: commandTimeout, commandType: commandType);
        }

        public async Task<SqlMapper.GridReader> QueryMultipleAsync(
            IDbConnection connection,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return await connection.QueryMultipleAsync(sql, param, transaction, commandTimeout, commandType);
        }

        public async Task<int> ExecuteAsync(
            IDbConnection connection,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null)
        {
            return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, commandType);
        }
    }
}

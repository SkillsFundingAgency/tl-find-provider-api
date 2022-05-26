using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Sfa.Tl.Find.Provider.Api.Interfaces;

public interface IDbContextWrapper
{
    IDbConnection CreateConnection();

    IDbTransaction BeginTransaction(IDbConnection connection);

    Task<IEnumerable<T>> QueryAsync<T>(
        IDbConnection connection,
        string sql,
        object param = null,
        IDbTransaction transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null);

    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(
        IDbConnection connection,
        string sql,
        Func<TFirst, TSecond, TReturn> map,
        object param = null,
        IDbTransaction transaction = null,
        string splitOn = "Id",
        int? commandTimeout = null,
        CommandType? commandType = null);

    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(
        IDbConnection connection,
        string sql,
        Func<TFirst, TSecond, TThird, TReturn> map,
        object param = null,
        IDbTransaction transaction = null,
        string splitOn = "Id",
        int? commandTimeout = null,
        CommandType? commandType = null);

    Task<T> ExecuteScalarAsync<T>(
        IDbConnection connection,
        string sql,
        object param = null,
        IDbTransaction transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null);
}
using System.Data;

namespace Sfa.Tl.Find.Provider.Application.Interfaces;

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

    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(
        IDbConnection connection,
        string sql,
        Func<TFirst, TSecond, TThird, TFourth, TReturn> map,
        object param = null,
        IDbTransaction transaction = null,
        string splitOn = "Id",
        int? commandTimeout = null,
        CommandType? commandType = null);

    Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(
        IDbConnection connection,
        string sql,
        Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map,
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
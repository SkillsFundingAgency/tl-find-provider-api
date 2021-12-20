using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Registry;
using Sfa.Tl.Find.Provider.Api.Extensions;
using Sfa.Tl.Find.Provider.Api.Interfaces;
using Sfa.Tl.Find.Provider.Api.Models;
using Sfa.Tl.Find.Provider.Api.Models.Configuration;

namespace Sfa.Tl.Find.Provider.Api.Data;

public class DbContextWrapper : IDbContextWrapper
{
    private readonly string _connectionString;
    private readonly IReadOnlyPolicyRegistry<string> _policyRegistry;
    private readonly ILogger<DbContextWrapper> _logger;

    public DbContextWrapper(
        IOptions<ConnectionStringSettings> connectionStringSettings,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        ILogger<DbContextWrapper> logger)
    {
        _policyRegistry = policyRegistry ?? throw new ArgumentNullException(nameof(policyRegistry));

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _connectionString = connectionStringSettings?.Value?.SqlConnectionString
                            ?? throw new ArgumentNullException(nameof(connectionStringSettings));
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
        CommandType? commandType = null)
    {
        var (retryPolicy, context) = GetRetryPolicy();
        return await retryPolicy
            .ExecuteAsync(async _ =>
                    await connection
                        .QueryAsync<T>(
                            sql,
                            param,
                            transaction,
                            commandTimeout,
                            commandType),
                context);
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
        var (retryPolicy, context) = GetRetryPolicy();
        return await retryPolicy
            .ExecuteAsync(async _ =>
                    await connection
                        .QueryAsync(
                            sql,
                            map,
                            param,
                            transaction,
                            splitOn: splitOn,
                            commandTimeout: commandTimeout,
                            commandType: commandType),
                context);
    }

    public async Task<T> ExecuteScalarAsync<T>(
        IDbConnection connection,
        string sql,
        object param = null,
        IDbTransaction transaction = null,
        int? commandTimeout = null,
        CommandType? commandType = null)
    {
        var (retryPolicy, context) = GetRetryPolicy();
        return await retryPolicy
            .ExecuteAsync(async _ =>
                    await connection.ExecuteScalarAsync<T>(
                        sql, param, transaction,
                        commandTimeout, commandType),
                context);
    }

    private (IAsyncPolicy, Context) GetRetryPolicy()
    {
        var retryPolicy =
            _policyRegistry
                .Get<IAsyncPolicy>(Constants.DapperRetryPolicyName)
            ?? Policy.NoOpAsync();

        //https://www.stevejgordon.co.uk/passing-an-ilogger-to-polly-policies
        var context = new Context($"{Guid.NewGuid()}",
            new Dictionary<string, object>
            {
                {
                    PolicyContextItems.Logger, _logger
                }
            });

        return (retryPolicy, context);
    }
}
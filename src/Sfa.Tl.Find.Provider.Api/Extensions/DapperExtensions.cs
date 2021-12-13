using System;
using Dapper;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging.Configuration;
using Polly;
using Polly.Retry;
using Sfa.Tl.Find.Provider.Api.Data;

namespace Sfa.Tl.Find.Provider.Api.Extensions
{
    public static class DapperExtensions
    {
        public static SqlMapper.ICustomQueryParameter AsTableValuedParameter<T>(
            this IEnumerable<T> enumerable,
            string typeName,
            IEnumerable<string> orderedColumnNames = null)
        {
            return enumerable.AsDataTable(orderedColumnNames).AsTableValuedParameter(typeName);
        }

        private const int RetryCount = 4;
        private static readonly Random Random = new();

        private static readonly IEnumerable<TimeSpan> RetryTimes = new[]
        {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(3)
        };

        //https://www.stevejgordon.co.uk/passing-an-ilogger-to-polly-policies

        private static readonly AsyncRetryPolicy RetryPolicy = Policy
            .Handle<SqlException>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            .Or<TimeoutException>()
            .OrInner<Win32Exception>(SqlServerTransientExceptionDetector.ShouldRetryOn)
            /*.WaitAndRetryAsync(
                RetryTimes,
                (exception, timeSpan, retryCount, context) =>
                {
                    //LogTo.Warning(
                    //    exception,
                    //    "WARNING: Error talking to ReportingDb, will retry after {RetryTimeSpan}. Retry attempt {RetryCount}",
                    //    timeSpan,
                    //    retryCount
                    //);
                });
            */
            .WaitAndRetryAsync(
                RetryCount,
                currentRetryNumber => TimeSpan.FromSeconds(Math.Pow(1.5, currentRetryNumber - 1)) + TimeSpan.FromMilliseconds(Random.Next(0, 100)),
                (currentException, 
                    currentSleepDuration, 
                    currentRetryNumber, 
                    context) =>
                {
                    //var logger1 = context.GetLogger();
                    var hasLogger = context.TryGetLogger(out var logger);


                    //currentContext.Se
                    //LogTo.Warning
#if DEBUG
                    Debug.WriteLine($"=== Attempt {currentRetryNumber} ===");
                    Debug.WriteLine(nameof(currentException) + ": " + currentException);
                    Debug.WriteLine(nameof(context) + ": " + context);
                    Debug.WriteLine(nameof(currentSleepDuration) + ": " + currentSleepDuration);
#endif
                });

        public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T>(
            this IDbConnection connection,
            string sql,
            object param = null,
            IDbTransaction transaction = null,
            int? commandTimeout = null,
            CommandType? commandType = null) =>
            await RetryPolicy
                .ExecuteAsync(async () =>
                    await connection
                        .QueryAsync<T>(
                            sql, 
                            param, 
                            transaction, 
                            commandTimeout, 
                            commandType));

        public static async Task<IEnumerable<TReturn>> QueryAsyncWithRetry<TFirst, TSecond, TThird, TReturn>(
            this IDbConnection connection, 
            string sql,
            Func<TFirst, TSecond, TThird, TReturn> map,
            object param = null,
            IDbTransaction transaction = null,
            string splitOn = "Id",
            int? commandTimeout = null,
            CommandType? commandType = null) =>
            await RetryPolicy
                .ExecuteAsync(async () => 
                    await connection
                        .QueryAsync<TFirst, TSecond, TThird, TReturn>(
                            sql, 
                            map, 
                            param,
                            transaction,
                            splitOn: splitOn, 
                            commandTimeout: commandTimeout, 
                            commandType: commandType));
    }
}

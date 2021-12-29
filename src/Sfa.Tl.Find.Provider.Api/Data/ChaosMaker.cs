#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Sfa.Tl.Find.Provider.Api.Data;

public static class ChaosMaker
{
    private static readonly Random Random = new();
    private static readonly bool IsRunningFromTest =
        AppDomain.CurrentDomain.GetAssemblies().Any(
            // ReSharper disable once StringLiteralTypo
            a => a.FullName!.ToLowerInvariant().StartsWith("xunit.runner"));

    public static void MakeChaos(int maxRandomValue = 0)
    {
        if (IsRunningFromTest || maxRandomValue == 0) return;

        switch (Random.Next(6))
        {
            case 1:
                throw CreateSqlException(49920);
            case 2:
                throw CreateSqlException(40613);
        }
    }

    private static SqlException CreateSqlException(int number)
    {
        Exception? innerEx = null;
        var c = typeof(SqlErrorCollection).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        var errors = (c[0].Invoke(null) as SqlErrorCollection)!;
        var errorList = (errors.GetType().GetField("_errors", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(errors) as List<object>)!;
        c = typeof(SqlError).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
        var nineC = c.FirstOrDefault(f => f.GetParameters().Length == 9)!;
        var sqlError = (nineC.Invoke(new object?[] { number, (byte)0, (byte)0, "", "", "", 0, (uint)0, innerEx }) as SqlError)!;
        errorList.Add(sqlError);

        return (Activator.CreateInstance(typeof(SqlException), BindingFlags.NonPublic | BindingFlags.Instance, null, new object?[] { "test", errors,
                innerEx, Guid.NewGuid() }, null) as SqlException)!;
    }
}

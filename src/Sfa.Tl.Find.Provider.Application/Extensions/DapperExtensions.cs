using Dapper;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class DapperExtensions
{
    public static SqlMapper.ICustomQueryParameter AsTableValuedParameter<T>(
        this IEnumerable<T> enumerable,
        string typeName,
        IEnumerable<string> orderedColumnNames = null)
    {
        return enumerable
            .AsDataTable(orderedColumnNames)
            .AsTableValuedParameter(typeName);
    }
}
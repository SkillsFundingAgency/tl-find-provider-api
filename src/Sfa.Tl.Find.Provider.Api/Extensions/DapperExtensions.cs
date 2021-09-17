using Dapper;
using System.Collections.Generic;

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
    }
}

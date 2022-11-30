using System.Data;
using System.Reflection;
using Dapper.Contrib.Extensions;

namespace Sfa.Tl.Find.Provider.Application.Extensions;

public static class EnumerableExtensions
{
    public static DataTable AsDataTable<T>(
        this IEnumerable<T> data,
        IEnumerable<string> orderedColumnNames = null)
    {
        var dataTable = new DataTable();
        if (typeof(T).IsValueType)
        {
            dataTable.Columns.Add("NONAME", typeof(T));
            foreach (var obj in data)
            {
                dataTable.Rows.Add(obj);
            }
        }
        else
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(w =>
                        w.CanRead &&
                        !w.HasAttribute<WriteAttribute>() //Not to be written via Dapper
            ).ToArray();

            var columnNames = (orderedColumnNames 
                               ?? properties.Select(s => s.Name)).ToArray();
            foreach (var name in columnNames)
            {
                var propertyInfo = properties.Single(s => s.Name.Equals(name));
                dataTable.Columns.Add(name,
                    propertyInfo.PropertyType.Name.Contains("Nullable") 
                        ? typeof(string) 
                        : propertyInfo.PropertyType);
            }

            foreach (var obj in data)
            {
                dataTable.Rows.Add(
                    columnNames.Select(s => properties.Single(s2 => s2.Name.Equals(s)).GetValue(obj))
                        .ToArray());
            }
        }

        return dataTable;
    }
}
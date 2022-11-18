﻿using System.ComponentModel.DataAnnotations;
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
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var readableProperties = properties.Where(w => 
                    w.CanRead && 
                    !w.HasAttribute<WriteAttribute>() //Not to be written via Dapper
            ).ToArray();

            var columnNames = (orderedColumnNames ?? readableProperties.Select(s => s.Name)).ToArray();
            foreach (var name in columnNames)
            {
                if (name == "ContactPreferenceType")
                {
                    var propertyInfo = readableProperties.Single(s => s.Name.Equals(name));
                    var column = new DataColumn
                    {
                        ColumnName = name,
                        DataType = propertyInfo.PropertyType.Name.Contains("Nullable") ? typeof(string) : propertyInfo.PropertyType
                    };
                    dataTable.Columns.Add(column);
                }
                else
                dataTable.Columns.Add(name, readableProperties.Single(s => s.Name.Equals(name)).PropertyType);
            }

            foreach (var obj in data)
            {
                dataTable.Rows.Add(
                    columnNames.Select(s => readableProperties.Single(s2 => s2.Name.Equals(s)).GetValue(obj))
                        .ToArray());
            }
        }

        return dataTable;
    }
}
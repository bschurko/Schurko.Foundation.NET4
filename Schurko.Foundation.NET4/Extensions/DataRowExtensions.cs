
using System;
using System.Data;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class DataRowExtensions
    {
        public static bool ContainsColumn(this DataRow dataRow, string column) => ContainsColumn(column, dataRow);

        public static bool ContainsColumn(this DataRow dataRow, Enum column) => ContainsColumn(Enum.GetName(column.GetType(), column), dataRow);

        private static bool ContainsColumn(string column, DataRow dataRow) => dataRow.Table.Columns.Contains(column);
    }
}

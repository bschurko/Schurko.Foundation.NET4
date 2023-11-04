
using System.Data;


#nullable enable
namespace Schurko.Foundation.Extensions
{
    public static class DataSetExtensions
    {
        public static bool HasData(this DataSet dsSet) => CalculateHasData(dsSet, 0);

        public static bool HasData(this DataSet dsSet, int tableNumber) => CalculateHasData(dsSet, tableNumber);

        private static bool CalculateHasData(DataSet dsSet, int tableNumber) => dsSet != null && dsSet.Tables.Count != 0 && dsSet.Tables.Count >= tableNumber + 1 && dsSet.Tables[tableNumber].Rows.Count > 0;
    }
}

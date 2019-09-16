using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Reports.MDSHA
{
    /// <summary>
    ///     This static class collects useful extension methods developed while
    ///     implementing the new (Spring/Summer 2014) MDSHA RoadCare reports.
    /// </summary>
    /// <remarks>
    ///     Many of these extension methods are useful beyond the reports
    ///     themselves. It would be worthwhile to collect such extension methods
    ///     into a more global, central location in the RoadCare solution
    ///     layout.
    /// </remarks>
    public static class Extensions
    {
        /// <summary>
        ///     Apply a border to a given EPPlus Border object.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="borderStyle"></param>
        /// <remarks>
        ///     This implementation reflects the idiomatic way to do this in
        ///     EPPlus.
        /// </remarks>
        public static void BorderAll(
            this Border @this,
            ExcelBorderStyle borderStyle)
        {
            @this.Left.Style = borderStyle;
            @this.Right.Style = borderStyle;
            @this.Top.Style = borderStyle;
            @this.Bottom.Style = borderStyle;
        }

        /// <summary>
        ///     Apply a border to a given EPPlus Range object.
        /// </summary>
        /// <param name="this"></param>
        /// <param name="borderStyle"></param>
        /// <remarks>
        ///     This implementation runs faster than <see cref="BorderAll" />,
        ///     and it applies borders to subranges in such a way that the same
        ///     all-border effect is achieved, but without actually bordering
        ///     all individual cells. NOTE: It assumes a strictly rectangular
        ///     range!
        /// </remarks>
        public static void BorderAllFaster(
            this ExcelRangeBase @this,
            ExcelBorderStyle borderStyle)
        {
            var fromRow = @this.Start.Row;
            var fromCol = @this.Start.Column;
            var toRow = @this.End.Row;
            var toCol = @this.End.Column;

            var numRows = toRow - fromRow + 1;
            var numCols = toCol - fromCol + 1;

            @this.Style.Border.BorderAround(borderStyle);

            for (var rowOffset = 1; rowOffset < numRows; rowOffset += 2)
            {
                var row = @this.Offset(rowOffset, 0, 1, numCols);
                row.Style.Border.BorderAround(borderStyle);
            }

            for (var colOffset = 1; colOffset < numCols; colOffset += 2)
            {
                var col = @this.Offset(0, colOffset, numRows, 1);
                col.Style.Border.BorderAround(borderStyle);
            }
        }

        /// <summary>
        ///     Similar to Enumerable.ElementAt, but accepts a sequence of
        ///     indexes at which to retrieve elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="indexes"></param>
        /// <returns>
        ///     the sequence of elements from the given indexes
        /// </returns>
        /// <remarks>
        ///     Note that the result sequence enumerates elements in the order
        ///     provided by <paramref name="indexes" />.
        /// </remarks>
        public static IEnumerable<T> ElementsAt<T>(
            this IEnumerable<T> @this,
            params int[] indexes)
        {
            foreach (var i in indexes)
            {
                yield return @this.ElementAt(i);
            }
        }

        /// <summary>
        ///     Constructs an "inversion" of a given indexable collection,
        ///     mapping elements to indexes rather than indexes to elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">an indexable collection</param>
        /// <returns>
        ///     a dictionary containing the element-to-index mapping
        /// </returns>
        /// <remarks>
        ///     Any duplicate element will map to the index of its last
        ///     occurrence in the input list.
        /// </remarks>
        public static Dictionary<T, int> GetInverseMapping<T>(this IList<T> @this) => @this.GetInverseMapping(null);

        /// <summary>
        ///     Constructs an "inversion" of a given indexable collection,
        ///     mapping elements to indexes rather than indexes to elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this">an indexable collection</param>
        /// <param name="comparer">determines equality of elements</param>
        /// <returns>
        ///     a dictionary containing the element-to-index mapping
        /// </returns>
        /// <remarks>
        ///     Any duplicate element will map to the index of its last
        ///     occurrence in the input list.
        /// </remarks>
        public static Dictionary<T, int> GetInverseMapping<T>(
            this IList<T> @this,
            IEqualityComparer<T> comparer)
        {
            var d = new Dictionary<T, int>(@this.Count, comparer);
            for (int i = 0; i < @this.Count; ++i)
            {
                d[@this[i]] = i;
            }

            return d;
        }

        /// <summary>
        ///     Determines whether the sequence is sorted in ascending order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <param name="compare">a comparison function</param>
        /// <returns>whether the sequence is sorted</returns>
        public static bool IsSortedBy<T>(
            this IEnumerable<T> @this,
            Comparison<T> compare)
        {
            var prev = @this.FirstOrDefault();
            foreach (var curr in @this.Skip(1))
            {
                if (compare(prev, curr) > 0)
                {
                    return false;
                }
                prev = curr;
            }
            return true;
        }

        /// <summary>
        ///     This is a simple, domain-unrelated method to "subtract one" from
        ///     a string that represents a number.
        /// </summary>
        /// <param name="this"></param>
        /// <returns>
        ///     a string representing one less than the number represented by
        ///     the argument string
        /// </returns>
        /// <remarks>
        ///     This implementation assumes that the string represents an Int32.
        ///     Otherwise, an Int32.Parse exception will be thrown.
        /// </remarks>
        public static string SubtractOne(this string @this) => (int.Parse(@this) - 1).ToString();

        /// <summary>
        ///     This extension finds all string-type columns in a DataTable and
        ///     trims the values.
        /// </summary>
        /// <param name="this"></param>
        public static void TrimStringFields(this DataTable @this)
        {
            var stringColumns =
                from DataColumn c in @this.Columns
                where c.DataType == typeof(string)
                select c.Ordinal;

            foreach (DataRow row in @this.Rows)
            {
                foreach (var ordinal in stringColumns)
                {
                    var stringField = row[ordinal] as string;
                    if (stringField != null)
                    {
                        row[ordinal] = stringField.Trim();
                    }
                }
            }
        }
    }
}

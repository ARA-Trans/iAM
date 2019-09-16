namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using DatabaseManager;
    using OfficeOpenXml;
    
    public partial class Report
    {
        /// <summary>
        ///     This nested class represents the content of a worksheet in a
        ///     report workbook.
        /// </summary>
        public abstract class Sheet : ISheet
        {
            private const double MaxColumnWidthFactor = 5;

            private const double ColumnWidthTweakForWrap = 2;

            public string Caption { get; protected set; }

            protected Report Report { get; private set; }

            public void AddTo(Report report)
            {
                this.Report = report;
                report.Sheets.Add(this);
            }

            public void Insert()
            {
                var book = this.Report.OutputPackage.Workbook;
                var sheet = book.Worksheets.Add(this.Caption);

                var items = this.GetContentItems();
                var ranges = this.GetItemRanges(items, sheet);

                // Insert all content
                Utilities.Zip(items, ranges, (item, range) => item.InsertAt(range));

                // Auto-fit all columns
                sheet.Cells.AutoFitColumns(
                    sheet.DefaultColWidth,
                    sheet.DefaultColWidth * MaxColumnWidthFactor);

                // Bump each content column to be a little wider, since
                // EPPlus auto-fit seems to be a little too tight when wrap
                // is used afterward
                for (var c = 1; c <= sheet.Dimension.End.Column; ++c)
                {
                    sheet.Column(c).Width += ColumnWidthTweakForWrap;
                }

                // Set wrap for all cells
                sheet.Cells.Style.WrapText = true;
            }

            public override string ToString()
            {
                return this.Caption;
            }

            protected abstract List<FormattedTable> GetContentItems();

            protected DataTable GetResultTable(string queryTemplate)
            {
                var queryString = string.Format(
                    queryTemplate,
                    this.Report.SimulationId);

                var queryResult = DBMgr.ExecuteQuery(queryString);
                var data = queryResult.Tables[0];

                data.TrimStringFields();

                return data;
            }

            private List<ExcelRangeBase> GetItemRanges(
                List<FormattedTable> items,
                ExcelWorksheet sheet)
            {
                var ranges = new List<ExcelRangeBase>();

                if (items.Count != 0)
                {
                    var firstItem = items.First();
                    var numRows = firstItem.RowCount;
                    var numCols = firstItem.ColumnCount;
                    ranges.Add(sheet.Cells.Offset(0, 0, numRows, numCols));
                }

                return items.Skip(1).Aggregate(ranges, this.AddNextRange);
            }

            private List<ExcelRangeBase> AddNextRange(
                List<ExcelRangeBase> ranges,
                FormattedTable nextItem)
            {
                var prevRange = ranges.Last();
                var prevRows = prevRange.End.Row - prevRange.Start.Row + 1;
                var nextRange = prevRange.Offset(
                    prevRows + 1,
                    0,
                    nextItem.RowCount,
                    nextItem.ColumnCount);

                ranges.Add(nextRange);

                return ranges;
            }
        }
    }
}

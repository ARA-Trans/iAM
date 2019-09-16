namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using OfficeOpenXml;

    public class FormattedTable
    {
        private readonly DataTable Table;

        private readonly List<RangeFormatting> Formattings;

        public FormattedTable(DataTable table, bool showHeaders)
        {
            this.Table = table;
            this.ShowHeaders = showHeaders;
            this.Formattings = new List<RangeFormatting>();
        }

        public FormattedTable(DataTable table) : this(table, true)
        {
        }

        public bool ShowHeaders { get; private set; }

        public int RowCount
        {
            get
            {
                return (this.ShowHeaders ? 1 : 0) + this.Table.Rows.Count;
            }
        }

        public int ColumnCount
        {
            get
            {
                return this.Table.Columns.Count;
            }
        }

        public void AddFormatting(RangeFormatting fmt)
        {
            this.Formattings.Add(fmt);
        }

        public void InsertAt(ExcelRangeBase range)
        {
            // Load data into cells first ...
            // --- There may be a slight performance hit with doing this before
            // formatting.
            range.LoadFromDataTable(this.Table, this.ShowHeaders);

            // ... then apply formatting.
            // --- This is second mainly so that table values themselves can be
            // modified or some kind of conditional formatting (not the kind
            // that's built-in with Excel) can be applied.
            var referenceRange = this.ShowHeaders ? range.Offset(1, 0) : range;
            foreach (var fmt in this.Formattings)
            {
                fmt.Apply(referenceRange);
            }
        }

        public void AddColumnRenaming(string oldName, string newName)
        {
            if (!this.ShowHeaders) return;

            var targetColumn = this.Table.Columns[oldName];
            if (targetColumn != null)
            {
                this.AddFormatting(new RangeFormatting(
                    range => range.Value = newName,
                    -1,
                    targetColumn.Ordinal));
            }
        }

        public void AddTableFormatting(bool autoFilter)
        {
            Action<ExcelRangeBase> a;
            if (autoFilter) a = Format.AfTable;
            else a = Format.Table;

            this.AddFormatting(new RangeFormatting(
                a,
                -1,
                0,
                1 + this.Table.Rows.Count,
                this.Table.Columns.Count));
        }
    }
}

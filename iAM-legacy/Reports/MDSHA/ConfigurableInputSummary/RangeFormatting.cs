namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System;
    using OfficeOpenXml;

    public class RangeFormatting
    {
        private readonly Action<ExcelRangeBase> Formatter;

        private readonly int RowOffset;

        private readonly int ColumnOffset;

        private readonly int RowCount;

        private readonly int ColumnCount;

        public RangeFormatting(
            Action<ExcelRangeBase> formatter,
            int rowOffset,
            int columnOffset,
            int rowCount,
            int columnCount)
        {
            // And another time when I wish we had Scala's parametric fields...
            this.Formatter = formatter;
            this.RowOffset = rowOffset;
            this.ColumnOffset = columnOffset;
            this.RowCount = rowCount;
            this.ColumnCount = columnCount;
        }

        public RangeFormatting(
            Action<ExcelRangeBase> formatter,
            int rowOffset,
            int columnOffset)
            : this(formatter, rowOffset, columnOffset, 1, 1)
        {
        }

        public RangeFormatting(Action<ExcelRangeBase> formatter)
            : this(formatter, 0, 0, 1, 1)
        {
        }

        public void Apply(ExcelRangeBase referenceRange)
        {
            var targetRange = referenceRange.Offset(
                this.RowOffset,
                this.ColumnOffset,
                this.RowCount,
                this.ColumnCount);

            this.Formatter(targetRange);
        }
    }
}

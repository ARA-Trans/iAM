namespace Reports.MDSHA.ConfigurableInputSummary
{
    using System.Drawing;
    using OfficeOpenXml;
    using OfficeOpenXml.Style;

    public static class Format
    {
        private static Color AraBgColor = ColorTranslator.FromHtml("#002E6C");

        private static Color AraFgColor = Color.White;

        public static void AllBorders(ExcelRangeBase range)
        {
            var cells = range.Worksheet.Cells;
            var fromRow = range.Start.Row;
            var fromCol = range.Start.Column;
            var toRow = range.End.Row;
            var toCol = range.End.Column;
            for (var r = fromRow; r <= toRow; ++r)
            {
                for (var c = fromCol; c <= toCol; ++c)
                {
                    cells[r, c]
                        .Style
                        .Border
                        .BorderAround(ExcelBorderStyle.Thin);
                }
            }
        }

        public static void AraBackground(ExcelRangeBase range)
        {
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(AraBgColor);
        }

        public static void AraForeground(ExcelRangeBase range)
        {
            range.Style.Font.Color.SetColor(AraFgColor);
        }

        public static void AutoFilter(ExcelRangeBase range)
        {
            range.AutoFilter = true;
        }

        public static void Bold(ExcelRangeBase range)
        {
            range.Style.Font.Bold = true;
        }

        public static void Currency(ExcelRangeBase range)
        {
            range.Style.Numberformat.Format =
                @"_($* #,##0_);_($* (#,##0);_($* ""-""??_);_(@_)";
        }

        public static void Heading(ExcelRangeBase range)
        {
            AraBackground(range);
            AraForeground(range);
            Bold(range);
        }

        public static void Table(ExcelRangeBase range)
        {
            var headings = range.Offset(
                0,
                0,
                1,
                range.End.Column - range.Start.Column + 1);

            Heading(headings);
            AllBorders(range);
        }

        public static void AfTable(ExcelRangeBase range)
        {
            Table(range);
            AutoFilter(range);
        }
    }
}

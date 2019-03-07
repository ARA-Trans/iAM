﻿using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCare.Services
{
    public static class ExcelHelper
    {
        /// <summary>
        /// Merge given cells
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="fromRow"></param>
        /// <param name="fromColumn"></param>
        /// <param name="toRow"></param>
        /// <param name="toColumn"></param>
        public static void MergeCells(ExcelWorksheet worksheet, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            using (var cells = worksheet.Cells[fromRow, fromColumn, toRow, toColumn])
            {
                cells.Merge = true;
                ApplyStyle(cells);
            }
        }

        /// <summary>
        /// Apply style to given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void ApplyStyle(ExcelRange cells)
        {
            cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cells.Style.WrapText = true;
            cells.Style.Font.Bold = true;
        }

        /// <summary>
        /// Apply border to given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void ApplyBorder(ExcelRange cells)
        {
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        }

        /// <summary>
        /// Set currency format for given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void SetCurrencyFormat(ExcelRange cells)
        {
            cells.Style.Numberformat.Format = "$###,###,##0.00";
        }
                
        /// <summary>
        /// Set custom currency format for given cells
        /// </summary>
        /// <param name="cells"></param>
        public static void SetCustomCurrencyFormat(ExcelRange cells)
        {
            cells.Style.Numberformat.Format = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
        }
    }
}
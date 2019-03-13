using BridgeCare.Models;
using OfficeOpenXml;
using System.Collections.Generic;

namespace BridgeCare.Services
{
    public static class BridgeWorkSummaryCommon
    {
        /// <summary>
        /// Add headers for sections
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="sectionName"></param>
        public static void AddHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName)
        {
            AddWorkTypeHeader(worksheet, currentCell);
            AddMergeSectionHeader(worksheet, sectionName, simulationYears.Count, currentCell);
            AddYearsHeaderRow(worksheet, simulationYears, currentCell);
        }

        /// <summary>
        /// Update current cell object
        /// </summary>
        /// <param name="currentCell"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public static void UpdateCurrentCell(CurrentCell currentCell, int row, int column)
        {
            currentCell.Row = row;
            currentCell.Column = column;
        }

        /// <summary>
        /// Common piece of code for work summary tab
        /// </summary>
        /// <param name="currentCell"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public static void SetRowColumns(CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            startRow = ++currentCell.Row;
            startColumn = 1;
            row = startRow;
            column = startColumn;
        }

        /// <summary>
        /// Add bridge headers for sections
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="sectionName"></param>
        /// <param name="showPrevYearHeader"></param>
        public static void AddBridgeHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName, bool showPrevYearHeader)
        {
            AddMergeBridgeSectionHeader(worksheet, sectionName, simulationYears.Count + 1, currentCell);
            AddBridgeYearsHeaderRow(worksheet, simulationYears, currentCell, showPrevYearHeader);
        }

        private static void AddMergeSectionHeader(ExcelWorksheet worksheet, string headerText, int yearsCount, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            worksheet.Cells[row, ++column].Value = headerText;
            var cells = worksheet.Cells[row, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row, column + yearsCount - 1);
            cells = worksheet.Cells[row, column, row, column + yearsCount - 1];
            ExcelHelper.ApplyBorder(cells);
            ++row;
            UpdateCurrentCell(currentCell, row, column);
        }

        private static void AddYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, column].Value = year;
                var cells = worksheet.Cells[row, column];
                ExcelHelper.ApplyStyle(cells);
                ExcelHelper.ApplyBorder(cells);
                column++;
            }
            currentCell.Column = column - 1;
        }

        private static void AddWorkTypeHeader(ExcelWorksheet worksheet, CurrentCell currentCell)
        {
            const string WorkTypeHeader = "Work Type";
            var row = currentCell.Row;
            var column = 1;
            worksheet.Cells[++row, column].Value = WorkTypeHeader;
            var cells = worksheet.Cells[row, column, row + 1, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.ApplyBorder(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row + 1, column);

            // Empty column
            column++;
            UpdateCurrentCell(currentCell, row, column);
        }
        
        private static void AddMergeBridgeSectionHeader(ExcelWorksheet worksheet, string headerText, int mergeColumns, CurrentCell currentCell)
        {
            var row = currentCell.Row + 1;
            var column = 1;
            worksheet.Cells[row, column].Value = headerText;
            var cells = worksheet.Cells[row, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row, column + mergeColumns);
            cells = worksheet.Cells[row, column, row, column + mergeColumns];
            ExcelHelper.ApplyBorder(cells);
            ++row;
            UpdateCurrentCell(currentCell, row, column);
        }

        private static void AddBridgeYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, bool showPrevYearHeader)
        {
            var row = currentCell.Row;
            var startColumn = currentCell.Column + 1;
            var column = startColumn;
            if (showPrevYearHeader)
            {
                worksheet.Cells[row, column].Value = simulationYears[0] - 1;
            }
            ++column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, column].Value = year;
                column++;
            }
            currentCell.Column = column - 1;
            var cells = worksheet.Cells[row, startColumn, row, currentCell.Column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.ApplyBorder(cells);
        }
    }
}
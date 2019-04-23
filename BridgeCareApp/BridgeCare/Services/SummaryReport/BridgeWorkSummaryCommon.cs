using BridgeCare.Models;
using OfficeOpenXml;
using System.Collections.Generic;

namespace BridgeCare.Services
{
    public class BridgeWorkSummaryCommon
    {
        private readonly ExcelHelper excelHelper;

        public BridgeWorkSummaryCommon(ExcelHelper excelHelper)
        {
            this.excelHelper = excelHelper;
        }

        /// <summary>
        /// Add headers for sections
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="sectionName"></param>
        public void AddHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName)
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
        public void UpdateCurrentCell(CurrentCell currentCell, int row, int column)
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
        public void SetRowColumns(CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
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
        public void AddBridgeHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName, bool showPrevYearHeader)
        {
            AddMergeBridgeSectionHeader(worksheet, sectionName, simulationYears.Count + 1, currentCell);
            AddBridgeYearsHeaderRow(worksheet, simulationYears, currentCell, showPrevYearHeader);
        }

        private void AddMergeSectionHeader(ExcelWorksheet worksheet, string headerText, int yearsCount, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            worksheet.Cells[row, ++column].Value = headerText;
            var cells = worksheet.Cells[row, column];
            excelHelper.ApplyStyle(cells);
            excelHelper.MergeCells(worksheet, row, column, row, column + yearsCount - 1);
            cells = worksheet.Cells[row, column, row, column + yearsCount - 1];
            excelHelper.ApplyBorder(cells);
            ++row;
            UpdateCurrentCell(currentCell, row, column);
        }

        private void AddYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            foreach (var year in simulationYears)
            {
                worksheet.Cells[row, column].Value = year;
                var cells = worksheet.Cells[row, column];
                excelHelper.ApplyStyle(cells);
                excelHelper.ApplyBorder(cells);
                column++;
            }
            currentCell.Column = column - 1;
        }

        private void AddWorkTypeHeader(ExcelWorksheet worksheet, CurrentCell currentCell)
        {
            const string WorkTypeHeader = "Work Type";
            var row = currentCell.Row;
            var column = 1;
            worksheet.Cells[++row, column].Value = WorkTypeHeader;
            var cells = worksheet.Cells[row, column, row + 1, column];
            excelHelper.ApplyStyle(cells);
            excelHelper.ApplyBorder(cells);
            excelHelper.MergeCells(worksheet, row, column, row + 1, column);

            // Empty column
            column++;
            UpdateCurrentCell(currentCell, row, column);
        }
        
        private void AddMergeBridgeSectionHeader(ExcelWorksheet worksheet, string headerText, int mergeColumns, CurrentCell currentCell)
        {
            var row = currentCell.Row + 1;
            var column = 1;
            worksheet.Cells[row, column].Value = headerText;
            var cells = worksheet.Cells[row, column];
            excelHelper.ApplyStyle(cells);
            excelHelper.MergeCells(worksheet, row, column, row, column + mergeColumns);
            cells = worksheet.Cells[row, column, row, column + mergeColumns];
            excelHelper.ApplyBorder(cells);
            ++row;
            UpdateCurrentCell(currentCell, row, column);
        }

        private void AddBridgeYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, bool showPrevYearHeader)
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
            excelHelper.ApplyStyle(cells);
            excelHelper.ApplyBorder(cells);
        }

        /// <summary>
        /// Initialize Good, Fair, Poor label cells (common to some sections.)
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="startRow"></param>
        /// <param name="startColumn"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public void InitializeLabelCells(ExcelWorksheet worksheet, CurrentCell currentCell, out int startRow, out int startColumn, out int row, out int column)
        {
            SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.Good;
            worksheet.Cells[row++, column].Value = Properties.Resources.Fair;
            worksheet.Cells[row++, column++].Value = Properties.Resources.Poor;
        }
    }
}
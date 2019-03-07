﻿using System;
using System.Collections.Generic;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class SummaryReportBridgeWorkSummary
    {
        public void Fill(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, BridgeCareContext dbContext)
        {            
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            FillCostOfCulvertWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillCostOfBridgeWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            // TODO ask why Total row is with same hard coded number 30000000 in excel?
            FillTotalBudgetSection(worksheet, currentCell, simulationYears);

            worksheet.Cells.AutoFitColumns();
        }

        private void FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "Total Budget");
            AddDetailsForTotalBudget(worksheet, simulationYears, currentCell);
        }

        private void AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = "Preservation";
            worksheet.Cells[row++, column].Value = "Construction";            
            worksheet.Cells[row++, column].Value = "Total";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                // Preservation                
                worksheet.Cells[row, column].Value = string.Empty;

                // Construction                
                worksheet.Cells[++row, column].Value = string.Empty;

                // Total
                worksheet.Cells[++row, column].Value = 30000000;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomCurrencyFormat(worksheet.Cells[startRow, fromColumn, row, column]);

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
        }

        private void FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");
            AddCostsOfBridgeWork(worksheet, simulationDataModels, simulationYears, currentCell);
        }

        private void AddCostsOfBridgeWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = "Latex";
            worksheet.Cells[row++, column].Value = "Epoxy";
            worksheet.Cells[row++, column].Value = "Large Bridge Preservation";
            worksheet.Cells[row++, column].Value = "Deck Replacement";
            worksheet.Cells[row++, column].Value = "Sub Rehab";
            worksheet.Cells[row++, column].Value = "Super Replacement";
            worksheet.Cells[row++, column].Value = "Large Bridge Rehab";
            worksheet.Cells[row++, column].Value = "Replacement";
            worksheet.Cells[row++, column].Value = "Bridge Total";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                // Latex
                var latexCost = CalculateCost(simulationDataModels, year, "Latex");
                worksheet.Cells[row, column].Value = latexCost;

                // Epoxy
                var epoxyCost = CalculateCost(simulationDataModels, year, "Epoxy");
                worksheet.Cells[++row, column].Value = epoxyCost;

                // Large Bridge Preservation
                var largeBridgePreservationCost = CalculateCost(simulationDataModels, year, "Large Bridge Preservation");
                worksheet.Cells[++row, column].Value = largeBridgePreservationCost;

                // Deck Replacement
                var deckReplacementCost = CalculateCost(simulationDataModels, year, "Deck Replacement");
                worksheet.Cells[++row, column].Value = deckReplacementCost;

                // Sub Rehab
                var subRehabCost = CalculateCost(simulationDataModels, year, "Sub Rehab");
                worksheet.Cells[++row, column].Value = subRehabCost;

                // Super Replacement
                var superReplacementCost = CalculateCost(simulationDataModels, year, "Superstructure Replacement");
                worksheet.Cells[++row, column].Value = superReplacementCost;

                // Large Bridge Rehab
                var largeBridgeRehabCost = CalculateCost(simulationDataModels, year, "Large Bridge Rehab");
                worksheet.Cells[++row, column].Value = largeBridgeRehabCost;

                // Replacement
                var replacementCost = CalculateCost(simulationDataModels, year, "Bridge Replacement");
                worksheet.Cells[++row, column].Value = replacementCost;

                // Bridge Total
                worksheet.Cells[++row, column].Value = latexCost + epoxyCost + largeBridgePreservationCost + deckReplacementCost + subRehabCost + superReplacementCost + largeBridgeRehabCost + replacementCost;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomCurrencyFormat(worksheet.Cells[startRow, fromColumn, row, column]);

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
        }

        private void FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            AddHeaders(worksheet, currentCell, simulationYears, "Cost of Culvert Work");
            AddCostsOfCulvertWork(worksheet, simulationDataModels, simulationYears, currentCell);
        }

        private void AddCostsOfCulvertWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = "Preservation";
            worksheet.Cells[row++, column].Value = "Rehabilitation";
            worksheet.Cells[row++, column].Value = "Replacement";
            worksheet.Cells[row++, column].Value = "Culvert Total";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                
                // Preservation
                var preservationCost = CalculateCost(simulationDataModels, year, "Culvert Preservation");
                worksheet.Cells[row, column].Value = preservationCost;
                
                // Rehabilitation
                var rehabilitationCost = CalculateCost(simulationDataModels, year, "Culvert Rehabilitation");
                worksheet.Cells[++row, column].Value = rehabilitationCost;
                
                // Replacement
                var replacementCost = CalculateCost(simulationDataModels, year, "Culvert Replacement");
                worksheet.Cells[++row, column].Value = replacementCost;                

                // Culvert Total
                worksheet.Cells[++row, column].Value = preservationCost + rehabilitationCost + replacementCost;                
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomCurrencyFormat(worksheet.Cells[startRow, fromColumn, row, column]);

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
        }

        private static double CalculateCost(List<SimulationDataModel> simulationDataModels, int year, string project)
        {
            double preservationCost = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && y.Project == project);
                preservationCost = preservationCost + (yearData != null ? yearData.Cost : 0);
            }

            return preservationCost;
        }

        private void AddHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName)
        {
            AddWorkTypeHeader(worksheet, currentCell);
            AddMergeSectionHeader(worksheet, sectionName, simulationYears.Count, currentCell);
            AddYearsHeaderRow(worksheet, simulationYears, currentCell);
        }

        private void AddMergeSectionHeader(ExcelWorksheet worksheet, string headerText, int yearsCount, CurrentCell currentCell)
        {
            var row = currentCell.Row;
            var column = currentCell.Column;
            worksheet.Cells[row, ++column].Value = headerText;
            var cells = worksheet.Cells[row, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row, column + yearsCount - 1);
            cells = worksheet.Cells[row, column, row, column + yearsCount - 1];
            ExcelHelper.ApplyBorder(cells);
            currentCell.Row = ++row;
            currentCell.Column = column;
        }

        private void AddYearsHeaderRow(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
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

        private void AddWorkTypeHeader(ExcelWorksheet worksheet, CurrentCell currentCell)
        {
            const string workTypeHeader = "Work Type";
            var row = currentCell.Row;
            var column = currentCell.Column;
            worksheet.Cells[++row, column].Value = workTypeHeader;
            var cells = worksheet.Cells[row, column, row + 1, column];
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.ApplyBorder(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row + 1, column);

            // Empty column
            column++;
            currentCell.Row = row;
            currentCell.Column = column;
        }
    }
}
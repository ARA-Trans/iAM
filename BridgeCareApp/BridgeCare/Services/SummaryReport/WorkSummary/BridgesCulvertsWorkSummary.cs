using BridgeCare.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace BridgeCare.Services
{
    public class BridgesCulvertsWorkSummary
    {
        private readonly BridgeWorkSummaryCommon bridgeWorkSummaryCommon;
        private readonly ExcelHelper excelHelper;
        private readonly BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper;

        public BridgesCulvertsWorkSummary(BridgeWorkSummaryCommon bridgeWorkSummaryCommon, ExcelHelper excelHelper, BridgeWorkSummaryComputationHelper bridgeWorkSummaryComputationHelper)
        {
            this.bridgeWorkSummaryCommon = bridgeWorkSummaryCommon;
            this.excelHelper = excelHelper;
            this.bridgeWorkSummaryComputationHelper = bridgeWorkSummaryComputationHelper;
        }

        /// <summary>
        /// Fill sections with bridges and culverts details
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="currentCell"></param>
        /// <param name="simulationYears"></param>
        /// <param name="simulationDataModels"></param>
        public void FillBridgesCulvertsWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, List<string> treatments)
        {
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillNumberOfCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, simulationDataModels, projectRowNumberModel, treatments);
            FillNumberOfBridgesWorkedOnSection(worksheet, currentCell, simulationYears, simulationDataModels, projectRowNumberModel, treatments);
            FillNumberOfBridgesCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, projectRowNumberModel, treatments);
        }

        private void FillNumberOfCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Culverts Worked on", "Culvert Work Type");
            AddCountsOfCulvertsWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel, treatments);
        }

        private void AddCountsOfCulvertsWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            foreach (var item in treatments)
            {
                if (item.ToLower().Contains("culvert"))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                double culvertTotalCount = 0;

                foreach (var item in treatments)
                {
                    if (item.ToLower().Contains("culvert"))
                    {
                        var culvertCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, item);
                        worksheet.Cells[row, column].Value = culvertCount;
                        projectRowNumberModel.TreatmentsCount.Add(item + "_" + year, row);
                        row++;
                        culvertTotalCount += culvertCount;
                    }
                }
                worksheet.Cells[row, column].Value = culvertTotalCount;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightSteelBlue);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillNumberOfBridgesWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges Worked on", "Bridge Work Type");
            AddCountsOfBridgesWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel, treatments);
        }

        private void AddCountsOfBridgesWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);

            foreach (var item in treatments)
            {
                if (!item.ToLower().Contains("culvert") && !item.ToLower().Contains("no treatment"))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                double nonCulvertTotalCount = 0;

                foreach (var item in treatments)
                {
                    if (!item.ToLower().Contains("culvert") && !item.ToLower().Contains("no treatment"))
                    {
                        var nonCulvertCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, item);
                        worksheet.Cells[row, column].Value = nonCulvertCount;
                        projectRowNumberModel.TreatmentsCount.Add(item + "_" + year, row);
                        row++;
                        nonCulvertTotalCount += nonCulvertCount;
                    }
                }
                worksheet.Cells[row, column].Value = nonCulvertTotalCount;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.SlateGray);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillNumberOfBridgesCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "Number of Bridges and Culverts Worked on", "Bridge and Cultvert Work Types");
            AddDetailsForNumberOfBridgesCulvertsWorkedOn(worksheet, currentCell, simulationYears, projectRowNumberModel, treatments);
        }

        private void AddDetailsForNumberOfBridgesCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel, List<string> treatments)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            foreach (var item in treatments)
            {
                if(!item.ToLower().Contains("no treatment"))
                {
                    worksheet.Cells[row++, column].Value = item;
                }
            }
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;
                var totalCount = 0;

                foreach (var item in treatments)
                {
                    var count = 0;
                    if (!item.ToLower().Contains("no treatment"))
                    {
                        count = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.TreatmentsCount[item + "_" + year], column].Value);
                        worksheet.Cells[row++, column].Value = count;
                        totalCount += count;
                    }
                }
                worksheet.Cells[row, column].Value = totalCount;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightBlue);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            excelHelper.ApplyColor(worksheet.Cells[row + 1, startColumn, row + 1, column], Color.DimGray);
        }
    }
}

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
        public void FillBridgesCulvertsWorkSummarySections(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillNumberOfCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, simulationDataModels, projectRowNumberModel);
            FillNumberOfBridgesWorkedOnSection(worksheet, currentCell, simulationYears, simulationDataModels, projectRowNumberModel);
            FillNumberOfBridgesCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, projectRowNumberModel);
        }

        private void FillNumberOfCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "# of Culverts Worked on");
            AddCountsOfCulvertsWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel);
        }

        private void AddCountsOfCulvertsWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.NoTreatment;
            worksheet.Cells[row++, column].Value = Properties.Resources.Preservation;
            worksheet.Cells[row++, column].Value = Properties.Resources.PreservationPoorFix;
            worksheet.Cells[row++, column].Value = Properties.Resources.Rehabilitation;
            worksheet.Cells[row++, column].Value = Properties.Resources.Replacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var noTreatmentCount = bridgeWorkSummaryComputationHelper.CalculateNoTreatmentCountForCulverts(simulationDataModels, year);
                worksheet.Cells[row, column].Value = noTreatmentCount;
                projectRowNumberModel.CulvertsNoTreatmentRow = row;

                int preservationPoorFixrow = row + 2;
                var preservationPoorFixCount = bridgeWorkSummaryComputationHelper.CalculatePreservationPoorFixCount(simulationDataModels, year);
                worksheet.Cells[preservationPoorFixrow, column].Value = preservationPoorFixCount;

                var preservationCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.CulvertPreservation) - preservationPoorFixCount;
                worksheet.Cells[++row, column].Value = preservationCount;
                projectRowNumberModel.CulvertsPreservationRow = row;

                row = preservationPoorFixrow + 1;
                var rehabilitationCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.CulvertRehabilitation);
                worksheet.Cells[row, column].Value = rehabilitationCount;
                projectRowNumberModel.CulvertsRehabilitationRow = row;

                var replacementCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.CulvertReplacement);
                worksheet.Cells[++row, column].Value = replacementCount;
                projectRowNumberModel.CulvertsReplacementRow = row;

                worksheet.Cells[++row, column].Value = preservationCount + preservationPoorFixCount + rehabilitationCount + replacementCount;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightSteelBlue);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillNumberOfBridgesWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "# of Bridges Worked on");
            AddCountsOfBridgesWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel);
        }

        private void AddCountsOfBridgesWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.NoTreatment;
            worksheet.Cells[row++, column].Value = Properties.Resources.Latex;
            worksheet.Cells[row++, column].Value = Properties.Resources.Epoxy;
            worksheet.Cells[row++, column].Value = Properties.Resources.LargeBridgePreservation;
            worksheet.Cells[row++, column].Value = Properties.Resources.DeckReplacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.SubRehab;
            worksheet.Cells[row++, column].Value = Properties.Resources.SuperReplacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.LargeBridgeRehab;
            worksheet.Cells[row++, column].Value = Properties.Resources.Replacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var noTreatmentCount = bridgeWorkSummaryComputationHelper.CalculateNoTreatmentCountForBridges(simulationDataModels, year);
                worksheet.Cells[row, column].Value = noTreatmentCount;
                projectRowNumberModel.BridgesNoTreatmentRow = row;

                var latexCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.Latex);
                worksheet.Cells[++row, column].Value = latexCount;
                projectRowNumberModel.BridgesPreservationRow = row;

                var epoxyCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.Epoxy);
                worksheet.Cells[++row, column].Value = epoxyCount;

                var largeBridgePreservationCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.LargeBridgePreservation);
                worksheet.Cells[++row, column].Value = largeBridgePreservationCount;

                var deckReplacementCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.DeckReplacement);
                worksheet.Cells[++row, column].Value = deckReplacementCount;
                projectRowNumberModel.BridgesRehabilitationRow = row;

                var subRehabCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.SubRehab);
                worksheet.Cells[++row, column].Value = subRehabCount;

                var superReplacementCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.SuperstructureReplacement);
                worksheet.Cells[++row, column].Value = superReplacementCount;

                var largeBridgeRehabCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.LargeBridgeRehab);
                worksheet.Cells[++row, column].Value = largeBridgeRehabCount;

                var replacementCount = bridgeWorkSummaryComputationHelper.CalculateCountByProject(simulationDataModels, year, Properties.Resources.BridgeReplacement);
                worksheet.Cells[++row, column].Value = replacementCount;
                projectRowNumberModel.BridgesReplacementRow = row;

                worksheet.Cells[++row, column].Value = latexCount + epoxyCount + largeBridgePreservationCount + deckReplacementCount + subRehabCount + superReplacementCount + largeBridgeRehabCount + replacementCount;
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.SlateGray);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillNumberOfBridgesCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            bridgeWorkSummaryCommon.AddHeaders(worksheet, currentCell, simulationYears, "# of Bridges and Culverts Worked on");
            AddDetailsForNumberOfBridgesCulvertsWorkedOn(worksheet, currentCell, simulationYears, projectRowNumberModel);
        }

        private void AddDetailsForNumberOfBridgesCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            int startRow, startColumn, row, column;
            bridgeWorkSummaryCommon.SetRowColumns(currentCell, out startRow, out startColumn, out row, out column);
            worksheet.Cells[row++, column].Value = Properties.Resources.NoTreatment;
            worksheet.Cells[row++, column].Value = Properties.Resources.Preservation;
            worksheet.Cells[row++, column].Value = Properties.Resources.Rehabilitation;
            worksheet.Cells[row++, column].Value = Properties.Resources.Replacement;
            worksheet.Cells[row++, column].Value = Properties.Resources.Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row, column].Value = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.CulvertsNoTreatmentRow, column].Value) + Convert.ToInt32(worksheet.Cells[projectRowNumberModel.BridgesNoTreatmentRow, column].Value);

                var bridgesRow = projectRowNumberModel.BridgesPreservationRow;
                var culvertsRow = projectRowNumberModel.CulvertsPreservationRow;
                worksheet.Cells[++row, column].Formula = "SUM(" + worksheet.Cells[bridgesRow, column, bridgesRow + 2, column] + "," + worksheet.Cells[culvertsRow, column, culvertsRow + 1, column] + ")";

                bridgesRow = projectRowNumberModel.BridgesRehabilitationRow;
                worksheet.Cells[++row, column].Formula = "SUM(" + worksheet.Cells[projectRowNumberModel.CulvertsRehabilitationRow, column] + "," + worksheet.Cells[bridgesRow, column, bridgesRow + 3, column] + ")";

                worksheet.Cells[++row, column].Value = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.CulvertsReplacementRow, column].Value) + Convert.ToInt32(worksheet.Cells[projectRowNumberModel.BridgesReplacementRow, column].Value);

                worksheet.Cells[++row, column].Formula = "SUM(" + worksheet.Cells[startRow + 1, column, startRow + 3, column] + ")";
            }
            excelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            excelHelper.ApplyColor(worksheet.Cells[startRow, fromColumn, row, column], Color.LightBlue);
            bridgeWorkSummaryCommon.UpdateCurrentCell(currentCell, ++row, column);
            excelHelper.ApplyColor(worksheet.Cells[row + 1, startColumn, row + 1, column], Color.DimGray);
        }
    }
}
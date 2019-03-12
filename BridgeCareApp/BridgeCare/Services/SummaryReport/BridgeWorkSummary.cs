using System;
using System.Collections.Generic;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class BridgeWorkSummary
    {
        #region constants
        const string NoTreatment = "No Treatment";
        const string Preservation = "Preservation";
        const string Rehabilitation = "Rehabilitation";
        const string Replacement = "Replacement";
        const string Latex = "Latex";
        const string Epoxy = "Epoxy";
        const string LargeBridgePreservation = "Large Bridge Preservation";
        const string DeckReplacement = "Deck Replacement";
        const string SubRehab = "Sub Rehab";
        const string SuperReplacement = "Super Replacement";
        const string LargeBridgeRehab = "Large Bridge Rehab";
        const string Total = "Total";
        const string Construction = "Construction";
        const string BridgeReplacement = "Bridge Replacement";
        const string SuperstructureReplacement = "Superstructure Replacement";
        const string CulvertPreservation = "Culvert Preservation";
        const string CulvertRehabilitation = "Culvert Rehabilitation";
        const string CulvertReplacement = "Culvert Replacement";
        const string BridgeCare = "Bridge Care";
        #endregion

        public void Fill(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, BridgeCareContext dbContext)
        {
            var currentCell = new CurrentCell { Row = 1, Column = 1 };

            var culvertTotalRow = FillCostOfCulvertWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            var bridgeTotalRow = FillCostOfBridgeWorkSection(worksheet, currentCell, simulationYears, simulationDataModels);
            // TODO ask why Total row is with same hard coded number 30000000 in excel?
            var budgetTotalRow = FillTotalBudgetSection(worksheet, currentCell, simulationYears);
            FillRemainingBudgetSection(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
            var projectRowNumberModel = new ProjectRowNumberModel();
            FillNumberOfCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, simulationDataModels, projectRowNumberModel);
            FillNumberOfBridgesWorkedOnSection(worksheet, currentCell, simulationYears, simulationDataModels, projectRowNumberModel);
            FillNumberOfBridgesCulvertsWorkedOnSection(worksheet, currentCell, simulationYears, projectRowNumberModel);
            FillPoorBridgeOnOfRateSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalPoorBridgesCountSection(worksheet, currentCell, simulationYears, simulationDataModels);
            FillTotalPoorBridgesDeckAreaSection(worksheet, currentCell, simulationYears, simulationDataModels);

            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();
        }

        private void FillTotalPoorBridgesDeckAreaSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Deck Area", true);
            AddDetailsForTotalPoorBridgesDeckArea(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private void AddDetailsForTotalPoorBridgesDeckArea(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row, column++].Value = BridgeCare;            
            worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesDeckArea(simulationDataModels, 0);
            foreach (var year in simulationYears)
            {
                column = ++column;
                worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesDeckArea(simulationDataModels, year);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, startColumn+1, row, column], "Number");
            UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillTotalPoorBridgesCountSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {            
            AddBridgeHeaders(worksheet, currentCell, simulationYears, "Total Poor Bridges Count", true);
            AddDetailsForTotalPoorBridgesCount(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private void AddDetailsForTotalPoorBridgesCount(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row, column++].Value = BridgeCare;            
            worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesCount(simulationDataModels, 0);
            foreach (var year in simulationYears)
            {                
                column = ++column;                
                worksheet.Cells[row, column].Value = BridgeWorkSummaryHelper.CalculateTotalPoorBridgesCount(simulationDataModels, year);
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillPoorBridgeOnOfRateSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            currentCell.Row = currentCell.Row + 2;
            AddBridgeHeaders(worksheet, currentCell, simulationYears, "Poor Bridge On and Off Rate", false);
            AddDetailsForPoorBridgeOnOfRate(worksheet, currentCell, simulationYears, simulationDataModels);
        }

        private void AddBridgeHeaders(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, string sectionName, bool showPrevYearHeader)
        {
            AddMergeBridgeSectionHeader(worksheet, sectionName, simulationYears.Count + 1, currentCell);
            AddBridgeYearsHeaderRow(worksheet, simulationYears, currentCell, showPrevYearHeader);
        }

        private void AddMergeBridgeSectionHeader(ExcelWorksheet worksheet, string headerText, int mergeColumns, CurrentCell currentCell)
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
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.ApplyBorder(cells);
        }

        private void AddDetailsForPoorBridgeOnOfRate(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = "# Bridge On";
            worksheet.Cells[row++, column].Value = "# Bridge Off";
            column++;            
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var count = BridgeWorkSummaryHelper.CalculatePoorBridgeCount(simulationDataModels, year, "On");
                worksheet.Cells[row, column].Value = count;

                count = BridgeWorkSummaryHelper.CalculatePoorBridgeCount(simulationDataModels, year, "Off");
                worksheet.Cells[++row, column].Value = count;                
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillNumberOfBridgesCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {            
            AddHeaders(worksheet, currentCell, simulationYears, "# of Bridges and Culverts Worked on");
            AddDetailsForNumberOfBridgesCulvertsWorkedOn(worksheet, currentCell, simulationYears, projectRowNumberModel);
        }

        private void AddDetailsForNumberOfBridgesCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = NoTreatment;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Rehabilitation;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = Total;
            column++;            
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
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillNumberOfBridgesWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel)
        {            
            AddHeaders(worksheet, currentCell, simulationYears, "# of Bridges Worked on");
            AddCountsOfBridgesWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel);
        }

        private void AddCountsOfBridgesWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = NoTreatment;
            worksheet.Cells[row++, column].Value = Latex;
            worksheet.Cells[row++, column].Value = Epoxy;
            worksheet.Cells[row++, column].Value = LargeBridgePreservation;
            worksheet.Cells[row++, column].Value = DeckReplacement;
            worksheet.Cells[row++, column].Value = SubRehab;
            worksheet.Cells[row++, column].Value = SuperReplacement;
            worksheet.Cells[row++, column].Value = LargeBridgeRehab;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = Total;
            column++;            
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var noTreatmentCount = BridgeWorkSummaryHelper.CalculateNoTreatmentCountForBridges(simulationDataModels, year);
                worksheet.Cells[row, column].Value = noTreatmentCount;
                projectRowNumberModel.BridgesNoTreatmentRow = row;

                var latexCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, Latex);
                worksheet.Cells[++row, column].Value = latexCount;
                projectRowNumberModel.BridgesPreservationRow = row;

                var epoxyCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, Epoxy);
                worksheet.Cells[++row, column].Value = epoxyCount;

                var largeBridgePreservationCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, LargeBridgePreservation);
                worksheet.Cells[++row, column].Value = largeBridgePreservationCount;

                var deckReplacementCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, DeckReplacement);
                worksheet.Cells[++row, column].Value = deckReplacementCount;
                projectRowNumberModel.BridgesRehabilitationRow = row;

                var subRehabCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, SubRehab);
                worksheet.Cells[++row, column].Value = subRehabCount;

                var superReplacementCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, SuperstructureReplacement);
                worksheet.Cells[++row, column].Value = superReplacementCount;

                var largeBridgeRehabCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, LargeBridgeRehab);
                worksheet.Cells[++row, column].Value = largeBridgeRehabCount;

                var replacementCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, BridgeReplacement);
                worksheet.Cells[++row, column].Value = replacementCount;
                projectRowNumberModel.BridgesReplacementRow = row;

                worksheet.Cells[++row, column].Value = latexCount + epoxyCount + largeBridgePreservationCount + deckReplacementCount + subRehabCount + superReplacementCount + largeBridgeRehabCount + replacementCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillNumberOfCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel)
        {
            AddHeaders(worksheet, currentCell, simulationYears, "# of Culverts Worked on");
            AddCountsOfCulvertsWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel);
        }

        private void AddCountsOfCulvertsWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = NoTreatment;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = "Preservation Poor Fix";
            worksheet.Cells[row++, column].Value = Rehabilitation;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = Total;
            column++;            
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var noTreatmentCount = BridgeWorkSummaryHelper.CalculateNoTreatmentCountForCulverts(simulationDataModels, year);
                worksheet.Cells[row, column].Value = noTreatmentCount;
                projectRowNumberModel.CulvertsNoTreatmentRow = row;

                int preservationPoorFixrow = row + 2;
                var preservationPoorFixCount = BridgeWorkSummaryHelper.CalculatePreservationPoorFixCount(simulationDataModels, year);
                worksheet.Cells[preservationPoorFixrow, column].Value = preservationPoorFixCount;

                var preservationCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, CulvertPreservation) - preservationPoorFixCount;
                worksheet.Cells[++row, column].Value = preservationCount;
                projectRowNumberModel.CulvertsPreservationRow = row;

                row = preservationPoorFixrow + 1;
                var rehabilitationCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, CulvertRehabilitation);
                worksheet.Cells[row, column].Value = rehabilitationCount;
                projectRowNumberModel.CulvertsRehabilitationRow = row;

                var replacementCount = BridgeWorkSummaryHelper.CalculateCountByProject(simulationDataModels, year, CulvertReplacement);
                worksheet.Cells[++row, column].Value = replacementCount;
                projectRowNumberModel.CulvertsReplacementRow = row;

                worksheet.Cells[++row, column].Value = preservationCount + preservationPoorFixCount + rehabilitationCount + replacementCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            UpdateCurrentCell(currentCell, ++row, column);
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {            
            AddHeaders(worksheet, currentCell, simulationYears, "Remaining Budget");
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Construction;
            worksheet.Cells[row++, column].Value = Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - (Convert.ToDouble(worksheet.Cells[culvertTotalRow, column].Value) + Convert.ToDouble(worksheet.Cells[bridgeTotalRow, column].Value));
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            UpdateCurrentCell(currentCell, ++row, column);
        }

        private int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {            
            AddHeaders(worksheet, currentCell, simulationYears, "Total Budget");
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears, currentCell);
            return budgetTotalRow;
        }

        private int AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            int budgetTotalRow = 0;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Construction;
            worksheet.Cells[row++, column].Value = Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                worksheet.Cells[row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = string.Empty;

                worksheet.Cells[++row, column].Value = 3000000;
                budgetTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            UpdateCurrentCell(currentCell, ++row, column);
            return budgetTotalRow;
        }

        private int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {            
            AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationDataModels, simulationYears, currentCell);
            return bridgeTotalRow;
        }

        private int AddCostsOfBridgeWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            int bridgeTotalRow = 0;
            worksheet.Cells[row++, column].Value = Latex;
            worksheet.Cells[row++, column].Value = Epoxy;
            worksheet.Cells[row++, column].Value = LargeBridgePreservation;
            worksheet.Cells[row++, column].Value = DeckReplacement;
            worksheet.Cells[row++, column].Value = SubRehab;
            worksheet.Cells[row++, column].Value = SuperReplacement;
            worksheet.Cells[row++, column].Value = LargeBridgeRehab;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = "Bridge Total";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var latexCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Latex);
                worksheet.Cells[row, column].Value = latexCost;

                var epoxyCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, Epoxy);
                worksheet.Cells[++row, column].Value = epoxyCost;

                var largeBridgePreservationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, LargeBridgePreservation);
                worksheet.Cells[++row, column].Value = largeBridgePreservationCost;

                var deckReplacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, DeckReplacement);
                worksheet.Cells[++row, column].Value = deckReplacementCost;

                var subRehabCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, SubRehab);
                worksheet.Cells[++row, column].Value = subRehabCost;

                var superReplacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, SuperstructureReplacement);
                worksheet.Cells[++row, column].Value = superReplacementCost;

                var largeBridgeRehabCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, LargeBridgeRehab);
                worksheet.Cells[++row, column].Value = largeBridgeRehabCost;

                var replacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, BridgeReplacement);
                worksheet.Cells[++row, column].Value = replacementCost;

                worksheet.Cells[++row, column].Value = latexCost + epoxyCost + largeBridgePreservationCost + deckReplacementCost + subRehabCost + superReplacementCost + largeBridgeRehabCost + replacementCost;
                bridgeTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            UpdateCurrentCell(currentCell, ++row, column);
            return bridgeTotalRow;
        }

        private int FillCostOfCulvertWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            AddHeaders(worksheet, currentCell, simulationYears, "Cost of Culvert Work");
            var culvertTotalRow = AddCostsOfCulvertWork(worksheet, simulationDataModels, simulationYears, currentCell);
            return culvertTotalRow;
        }

        private int AddCostsOfCulvertWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;            
            var startColumn = 1;
            var column = startColumn;
            int culvertTotalRow = 0;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Rehabilitation;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = "Culvert Total";
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                var preservationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, CulvertPreservation);
                worksheet.Cells[row, column].Value = preservationCost;

                var rehabilitationCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, CulvertRehabilitation);
                worksheet.Cells[++row, column].Value = rehabilitationCost;

                var replacementCost = BridgeWorkSummaryHelper.CalculateCost(simulationDataModels, year, CulvertReplacement);
                worksheet.Cells[++row, column].Value = replacementCost;

                worksheet.Cells[++row, column].Value = preservationCost + rehabilitationCost + replacementCost;
                culvertTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");
            UpdateCurrentCell(currentCell, ++row, column);
            return culvertTotalRow;
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
                ExcelHelper.ApplyStyle(cells);
                ExcelHelper.ApplyBorder(cells);
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
            ExcelHelper.ApplyStyle(cells);
            ExcelHelper.ApplyBorder(cells);
            ExcelHelper.MergeCells(worksheet, row, column, row + 1, column);

            // Empty column
            column++;
            UpdateCurrentCell(currentCell, row, column);
        }

        private static void UpdateCurrentCell(CurrentCell currentCell, int row, int column)
        {
            currentCell.Row = row;
            currentCell.Column = column;
        }
    }
}
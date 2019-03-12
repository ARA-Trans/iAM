using System;
using System.Collections.Generic;
using BridgeCare.Models;
using OfficeOpenXml;

namespace BridgeCare.Services
{
    public class SummaryReportBridgeWorkSummary
    {
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
            
            worksheet.Calculate();
            worksheet.Cells.AutoFitColumns();
        }

        private void FillNumberOfBridgesCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "# of Bridges and Culverts Worked on");
            AddDetailsForNumberOfBridgesCulvertsWorkedOn(worksheet, currentCell, simulationYears, projectRowNumberModel);
        }

        private void AddDetailsForNumberOfBridgesCulvertsWorkedOn(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, ProjectRowNumberModel projectRowNumberModel)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
            var startColumn = 1;
            var column = startColumn;
            worksheet.Cells[row++, column].Value = NoTreatment;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = Rehabilitation;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = Total;

            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                // No Treatment                
                worksheet.Cells[row, column].Value = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.CulvertsNoTreatmentRow, column].Value) + Convert.ToInt32(worksheet.Cells[projectRowNumberModel.BridgesNoTreatmentRow, column].Value);

                // Preservation  
                var bridgesRow = projectRowNumberModel.BridgesPreservationRow;
                var culvertsRow = projectRowNumberModel.CulvertsPreservationRow;
                worksheet.Cells[++row, column].Formula = "SUM(" + worksheet.Cells[bridgesRow, column, bridgesRow + 2, column] + "," + worksheet.Cells[culvertsRow, column, culvertsRow + 1, column] + ")";

                // Rehabilitation  
                bridgesRow = projectRowNumberModel.BridgesRehabilitationRow;
                worksheet.Cells[++row, column].Formula = "SUM(" + worksheet.Cells[projectRowNumberModel.CulvertsRehabilitationRow, column] + "," + worksheet.Cells[bridgesRow, column, bridgesRow + 3, column] + ")";

                // Replacement                
                worksheet.Cells[++row, column].Value = Convert.ToInt32(worksheet.Cells[projectRowNumberModel.CulvertsReplacementRow, column].Value) + Convert.ToInt32(worksheet.Cells[projectRowNumberModel.BridgesReplacementRow, column].Value);

                // Total
                worksheet.Cells[++row, column].Formula = "SUM(" + worksheet.Cells[startRow + 1, column, startRow + 3, column] + ")";
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);            

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
        }

        private void FillNumberOfBridgesWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "# of Bridges Worked on");
            AddCountsOfBridgesWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel);
        }

        private void AddCountsOfBridgesWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
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
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                // No Treatment
                var noTreatmentCount = CalculateNoTreatmentCountForBridges(simulationDataModels, year);
                worksheet.Cells[row, column].Value = noTreatmentCount;
                projectRowNumberModel.BridgesNoTreatmentRow = row;

                // Latex
                var latexCount = CalculateCountByProject(simulationDataModels, year, Latex);
                worksheet.Cells[++row, column].Value = latexCount;
                projectRowNumberModel.BridgesPreservationRow = row;

                // Epoxy                
                var epoxyCount = CalculateCountByProject(simulationDataModels, year, Epoxy);
                worksheet.Cells[++row, column].Value = epoxyCount;

                // Large Bridge Preservation                
                var largeBridgePreservationCount = CalculateCountByProject(simulationDataModels, year, LargeBridgePreservation);
                worksheet.Cells[++row, column].Value = largeBridgePreservationCount;

                // Deck Replacement           
                var deckReplacementCount = CalculateCountByProject(simulationDataModels, year, DeckReplacement);
                worksheet.Cells[++row, column].Value = deckReplacementCount;
                projectRowNumberModel.BridgesRehabilitationRow = row;

                // Sub Rehab                
                var subRehabCount = CalculateCountByProject(simulationDataModels, year, SubRehab);
                worksheet.Cells[++row, column].Value = subRehabCount;

                // Super Replacement                
                var superReplacementCount = CalculateCountByProject(simulationDataModels, year, SuperstructureReplacement);
                worksheet.Cells[++row, column].Value = superReplacementCount;

                // Large Bridge Rehab                
                var largeBridgeRehabCount = CalculateCountByProject(simulationDataModels, year, LargeBridgeRehab);
                worksheet.Cells[++row, column].Value = largeBridgeRehabCount;

                // Replacement                
                var replacementCount = CalculateCountByProject(simulationDataModels, year, BridgeReplacement);
                worksheet.Cells[++row, column].Value = replacementCount;
                projectRowNumberModel.BridgesReplacementRow = row;

                // Total
                worksheet.Cells[++row, column].Value = latexCount + epoxyCount + largeBridgePreservationCount + deckReplacementCount + subRehabCount + superReplacementCount + largeBridgeRehabCount + replacementCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
        }

        private int CalculateNoTreatmentCountForBridges(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == NoTreatment && y.CulvD.Equals("N"))).Count;
        }

        private void FillNumberOfCulvertsWorkedOnSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels, ProjectRowNumberModel projectRowNumberModel)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "# of Culverts Worked on");
            AddCountsOfCulvertsWorkedOn(worksheet, simulationDataModels, simulationYears, currentCell, projectRowNumberModel);            
        }

        private void AddCountsOfCulvertsWorkedOn(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell, ProjectRowNumberModel projectRowNumberModel)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
            var startColumn = 1;
            var column = startColumn;            
            worksheet.Cells[row++, column].Value = NoTreatment;
            worksheet.Cells[row++, column].Value = Preservation;
            worksheet.Cells[row++, column].Value = "Preservation Poor Fix";
            worksheet.Cells[row++, column].Value = Rehabilitation;
            worksheet.Cells[row++, column].Value = Replacement;
            worksheet.Cells[row++, column].Value = Total;
            column++;
            var fromColumn = column + 1;
            foreach (var year in simulationYears)
            {
                row = startRow;
                column = ++column;

                // No Treatment
                var noTreatmentCount = CalculateNoTreatmentCountForCulverts(simulationDataModels, year);
                worksheet.Cells[row, column].Value = noTreatmentCount;
                projectRowNumberModel.CulvertsNoTreatmentRow = row;

                // Preservation Poor Fix
                int preservationPoorFixrow = row + 2;
                var preservationPoorFixCount = CalculatePreservationPoorFixCount(simulationDataModels, year);
                worksheet.Cells[preservationPoorFixrow, column].Value = preservationPoorFixCount;

                // Preservation
                var preservationCount = CalculateCountByProject(simulationDataModels, year, CulvertPreservation) - preservationPoorFixCount;
                worksheet.Cells[++row, column].Value = preservationCount;
                projectRowNumberModel.CulvertsPreservationRow = row;

                // Rehabilitation
                row = preservationPoorFixrow + 1;
                var rehabilitationCount = CalculateCountByProject(simulationDataModels, year, CulvertRehabilitation);
                worksheet.Cells[row, column].Value = rehabilitationCount;
                projectRowNumberModel.CulvertsRehabilitationRow = row;

                // Replacement
                 var replacementCount = CalculateCountByProject(simulationDataModels, year, CulvertReplacement);
                worksheet.Cells[++row, column].Value = replacementCount;
                projectRowNumberModel.CulvertsReplacementRow = row;

                // Total
                worksheet.Cells[++row, column].Value = preservationCount + preservationPoorFixCount + rehabilitationCount + replacementCount;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);            

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;            
        }

        private int CalculatePreservationPoorFixCount(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Culv == "Y" && y.SD == "N")).Count;
        }

        private int CalculateNoTreatmentCountForCulverts(List<SimulationDataModel> simulationDataModels, int year)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == NoTreatment && !y.CulvD.Equals("N"))).Count;
        }

        private int CalculateCountByProject(List<SimulationDataModel> simulationDataModels, int year, string project)
        {
            return simulationDataModels.FindAll(s => s.YearsData.Exists(y => y.Year == year && y.Project == project)).Count;
        }

        private void FillRemainingBudgetSection(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "Remaining Budget");
            AddDetailsForRemainingBudget(worksheet, simulationYears, currentCell, culvertTotalRow, bridgeTotalRow, budgetTotalRow);
        }

        private void AddDetailsForRemainingBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell, int culvertTotalRow, int bridgeTotalRow, int budgetTotalRow)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
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

                // Preservation                
                worksheet.Cells[row, column].Value = string.Empty;

                // Construction                
                worksheet.Cells[++row, column].Value = string.Empty;

                // Total
                worksheet.Cells[++row, column].Value = Convert.ToDouble(worksheet.Cells[budgetTotalRow, column].Value) - (Convert.ToDouble(worksheet.Cells[culvertTotalRow, column].Value) + Convert.ToDouble(worksheet.Cells[bridgeTotalRow, column].Value));
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);                        
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");            

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;            
        }

        private int FillTotalBudgetSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "Total Budget");
            var budgetTotalRow = AddDetailsForTotalBudget(worksheet, simulationYears, currentCell);
            return budgetTotalRow;
        }

        private int AddDetailsForTotalBudget(ExcelWorksheet worksheet, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
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

                // Preservation                
                worksheet.Cells[row, column].Value = string.Empty;

                // Construction                
                worksheet.Cells[++row, column].Value = string.Empty;

                // Total
                worksheet.Cells[++row, column].Value = 3000000;
                budgetTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
            return budgetTotalRow;
        }

        private int FillCostOfBridgeWorkSection(ExcelWorksheet worksheet, CurrentCell currentCell, List<int> simulationYears, List<SimulationDataModel> simulationDataModels)
        {
            currentCell.Column = 1;
            AddHeaders(worksheet, currentCell, simulationYears, "Cost of Bridge Work");
            var bridgeTotalRow = AddCostsOfBridgeWork(worksheet, simulationDataModels, simulationYears, currentCell);
            return bridgeTotalRow;
        }

        private int AddCostsOfBridgeWork(ExcelWorksheet worksheet, List<SimulationDataModel> simulationDataModels, List<int> simulationYears, CurrentCell currentCell)
        {
            var startRow = ++currentCell.Row;
            var row = startRow;
            // Headers in column 1
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

                // Latex
                var latexCost = CalculateCost(simulationDataModels, year, Latex);
                worksheet.Cells[row, column].Value = latexCost;

                // Epoxy
                var epoxyCost = CalculateCost(simulationDataModels, year, Epoxy);
                worksheet.Cells[++row, column].Value = epoxyCost;

                // Large Bridge Preservation
                var largeBridgePreservationCost = CalculateCost(simulationDataModels, year, LargeBridgePreservation);
                worksheet.Cells[++row, column].Value = largeBridgePreservationCost;

                // Deck Replacement
                var deckReplacementCost = CalculateCost(simulationDataModels, year, DeckReplacement);
                worksheet.Cells[++row, column].Value = deckReplacementCost;

                // Sub Rehab
                var subRehabCost = CalculateCost(simulationDataModels, year, SubRehab);
                worksheet.Cells[++row, column].Value = subRehabCost;

                // Super Replacement
                var superReplacementCost = CalculateCost(simulationDataModels, year, SuperstructureReplacement);
                worksheet.Cells[++row, column].Value = superReplacementCost;

                // Large Bridge Rehab
                var largeBridgeRehabCost = CalculateCost(simulationDataModels, year, LargeBridgeRehab);
                worksheet.Cells[++row, column].Value = largeBridgeRehabCost;

                // Replacement
                var replacementCost = CalculateCost(simulationDataModels, year, BridgeReplacement);
                worksheet.Cells[++row, column].Value = replacementCost;

                // Bridge Total
                worksheet.Cells[++row, column].Value = latexCost + epoxyCost + largeBridgePreservationCost + deckReplacementCost + subRehabCost + superReplacementCost + largeBridgeRehabCost + replacementCost;
                bridgeTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
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
            // Headers in column 1
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

                // Preservation
                var preservationCost = CalculateCost(simulationDataModels, year, CulvertPreservation);
                worksheet.Cells[row, column].Value = preservationCost;

                // Rehabilitation
                var rehabilitationCost = CalculateCost(simulationDataModels, year, CulvertRehabilitation);
                worksheet.Cells[++row, column].Value = rehabilitationCost;

                // Replacement
                var replacementCost = CalculateCost(simulationDataModels, year, CulvertReplacement);
                worksheet.Cells[++row, column].Value = replacementCost;

                // Culvert Total
                worksheet.Cells[++row, column].Value = preservationCost + rehabilitationCost + replacementCost;
                culvertTotalRow = row;
            }
            ExcelHelper.ApplyBorder(worksheet.Cells[startRow, startColumn, row, column]);
            ExcelHelper.SetCustomFormat(worksheet.Cells[startRow, fromColumn, row, column], "NegativeCurrency");

            // Empty row
            currentCell.Row = ++row;
            currentCell.Column = column;
            return culvertTotalRow;
        }

        private static double CalculateCost(List<SimulationDataModel> simulationDataModels, int year, string project)
        {
            double cost = 0;
            foreach (var simulationDataModel in simulationDataModels)
            {
                var yearData = simulationDataModel.YearsData.Find(y => y.Year == year && y.Project == project);
                cost = cost + (yearData != null ? yearData.Cost : 0);
            }

            return cost;
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
            const string WorkTypeHeader = "Work Type";
            var row = currentCell.Row;
            var column = currentCell.Column;
            worksheet.Cells[++row, column].Value = WorkTypeHeader;
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
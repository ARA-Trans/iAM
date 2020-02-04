using System;
using System.Collections.Generic;
using System.Drawing;
using BridgeCare.Interfaces;
using BridgeCare.Interfaces.CriteriaDrivenBudgets;
using BridgeCare.Models;
using BridgeCare.Models.CriteriaDrivenBudgets;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace BridgeCare.Services.SummaryReport
{
    public class SummaryReportParameters
    {
        private readonly BridgeCareContext db;
        private readonly ISimulationAnalysis analysisData;
        private readonly IInvestmentLibrary getInflationRate;
        private readonly ExcelHelper excelHelper;
        private readonly IPriority getPriorities;
        private readonly ICriteriaDrivenBudgets budgetCriteria;

        public SummaryReportParameters(ISimulationAnalysis simulationAnalysis, IInvestmentLibrary inflationRate,
            ExcelHelper excelHelper, IPriority priorities, ICriteriaDrivenBudgets budget,  BridgeCareContext db)
        {
            analysisData = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            getInflationRate = inflationRate ?? throw new ArgumentNullException(nameof(inflationRate));
            this.excelHelper = excelHelper;
            getPriorities = priorities ?? throw new ArgumentNullException(nameof(priorities));
            budgetCriteria = budget ?? throw new ArgumentNullException(nameof(budget));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        internal void Fill(ExcelWorksheet worksheet, SimulationModel simulationModel)
        {
            var simulationId = simulationModel.SimulationId;
            var investmentPeriod = analysisData.GetSimulationAnalysis(simulationId, db);
            var inflationAndInvestments = getInflationRate.GetSimulationInvestmentLibrary(simulationId, db);
            var priorities = getPriorities.GetSimulationPriorityLibrary(simulationId, db).Priorities;
            var criterias = budgetCriteria.GetCriteriaDrivenBudgets(simulationId, db);

            // Simulation Name format
            excelHelper.MergeCells(worksheet, 1, 1, 1, 2);
            excelHelper.MergeCells(worksheet, 1, 3, 1, 10);

            worksheet.Cells["A1:B1"].Value = "Simulation Name";
            worksheet.Cells["C1:J1"].Value = simulationModel.SimulationName;
            excelHelper.ApplyBorder(worksheet.Cells[1, 1, 1, 10]);
            // End of Simulation Name format

            FillStaticData(worksheet);

            FillSimulationDetails(worksheet, investmentPeriod, inflationAndInvestments.InflationRate);
            FillAnalysisDetails(worksheet, investmentPeriod);
            FillJurisdictionCriteria(worksheet, investmentPeriod.Criteria);
            FillPriorities(worksheet, priorities);
            FillInvestmentAndBudgetCriteria(worksheet, inflationAndInvestments, criterias);
            worksheet.Cells.AutoFitColumns(50);
        }

        private void FillStaticData(ExcelWorksheet worksheet)
        {
            worksheet.Cells["A3:B3"].Value = "BridgeCare Rules Creator:";
            excelHelper.MergeCells(worksheet, 3, 1, 3, 2);
            worksheet.Cells["C3"].Value = "Central Office";
            worksheet.Cells["A4:B4"].Value = "BridgeCare Rules Date:";
            worksheet.Cells["C4"].Value = "10/25/2019";
            excelHelper.MergeCells(worksheet, 4, 1, 4, 2);
            excelHelper.ApplyBorder(worksheet.Cells[3, 1, 4, 2]);

            excelHelper.MergeCells(worksheet, 6, 1, 6, 2);
            worksheet.Cells["A6:B6"].Value = "NHS";
            worksheet.Cells["A7"].Value = "NHS";
            worksheet.Cells["A8"].Value = "Non-NHS";
            var flag = worksheet.DataValidations.AddListValidation("B7");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["B7"].Value = "Y";
            flag = worksheet.DataValidations.AddListValidation("B8");
            flag.Formula.Values.Add("Y");
            worksheet.Cells["B8"].Value = "Y";
            flag.Formula.Values.Add("N");
            excelHelper.ApplyBorder(worksheet.Cells[6, 1, 8, 2]);

            excelHelper.MergeCells(worksheet, 10, 1, 10, 2);
            worksheet.Cells["A10:B10"].Value = "6A19 BPN";
            worksheet.Cells["A11"].Value = "1";
            worksheet.Cells["A12"].Value = "2";
            worksheet.Cells["A13"].Value = "3";
            worksheet.Cells["A14"].Value = "4";
            worksheet.Cells["A15"].Value = "H";

            flag = worksheet.DataValidations.AddListValidation("B11");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["B11"].Value = "Y";
            flag = worksheet.DataValidations.AddListValidation("B12");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("B13");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("B14");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["B12"].Value = "Y";
            worksheet.Cells["B13"].Value = "Y";
            worksheet.Cells["B14"].Value = "Y";
            flag = worksheet.DataValidations.AddListValidation("B15");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            worksheet.Cells["B15"].Value = "N";

            worksheet.Cells["A16"].Value = "L";
            worksheet.Cells["A17"].Value = "T";
            worksheet.Cells["A18"].Value = "D";
            worksheet.Cells["A19"].Value = "N";
            worksheet.Cells["A20"].Value = "Blank";

            flag = worksheet.DataValidations.AddListValidation("B16");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("B17");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("B18");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("B19");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("B20");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["B16"].Value = "Y";
            worksheet.Cells["B17"].Value = "N";
            worksheet.Cells["B18"].Value = "N";
            worksheet.Cells["B19"].Value = "Y";
            worksheet.Cells["B20"].Value = "Y";
            excelHelper.ApplyBorder(worksheet.Cells[10, 1, 20, 2]);

            excelHelper.MergeCells(worksheet, 23, 1, 23, 2);
            worksheet.Cells["A23:B23"].Value = "Bridge Length";
            worksheet.Cells["A24"].Value = "8-20";
            worksheet.Cells["A25"].Value = "NBIS Length";

            flag = worksheet.DataValidations.AddListValidation("B24");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("B25");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["B24"].Value = "Y";
            worksheet.Cells["B25"].Value = "Y";

            excelHelper.MergeCells(worksheet, 27, 1, 27, 2);
            worksheet.Cells["A27:B27"].Value = "Status";
            worksheet.Cells["A28"].Value = "Open";
            worksheet.Cells["A29"].Value = "Closed";
            worksheet.Cells["A30"].Value = "P3";
            worksheet.Cells["A31"].Value = "Posted";

            flag = worksheet.DataValidations.AddListValidation("B28");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("B29");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("B30");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("B31");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["B28"].Value = "Y";
            worksheet.Cells["B29"].Value = "N";
            worksheet.Cells["B30"].Value = "N";
            worksheet.Cells["B31"].Value = "Y";
            excelHelper.ApplyBorder(worksheet.Cells[23, 1, 31, 2]);

            excelHelper.MergeCells(worksheet, 14, 4, 14, 6);
            worksheet.Cells["D14:F14"].Value = "5A21 Owner";

            excelHelper.MergeCells(worksheet, 15, 4, 15, 5, false);
            worksheet.Cells["D15:E15"].Value = "01 - State Highway Agency";
            excelHelper.MergeCells(worksheet, 16, 4, 16, 5, false);
            worksheet.Cells["D16:E16"].Value = "02 - County Highway Agency";
            excelHelper.MergeCells(worksheet, 17, 4, 17, 5, false);
            worksheet.Cells["D17:E17"].Value = "03 - Town or Township Highway Agency";
            excelHelper.MergeCells(worksheet, 18, 4, 18, 5, false);
            worksheet.Cells["D18:E18"].Value = "04 - City, Municipal Highway Agency or Borough";
            excelHelper.MergeCells(worksheet, 19, 4, 19, 5, false);
            worksheet.Cells["D19:E19"].Value = "11 - State Park, Forest or Reservation Agency";
            excelHelper.MergeCells(worksheet, 20, 4, 20, 5, false);
            worksheet.Cells["D20:E20"].Value = "12 - Local Park, Forest or Reservation Agency";
            excelHelper.MergeCells(worksheet, 21, 4, 21, 5, false);
            worksheet.Cells["D21:E21"].Value = "21 - Other State Agency";
            excelHelper.MergeCells(worksheet, 22, 4, 22, 5, false);
            worksheet.Cells["D22:E22"].Value = "25 - Other local Agency";
            excelHelper.MergeCells(worksheet, 23, 4, 23, 5, false);
            worksheet.Cells["D23:E23"].Value = "26 - Private (Other than Railroad)";
            excelHelper.MergeCells(worksheet, 24, 4, 24, 5, false);
            worksheet.Cells["D24:E24"].Value = "27 - Railroad";
            excelHelper.MergeCells(worksheet, 25, 4, 25, 5, false);
            worksheet.Cells["D25:E25"].Value = "31 - State Toll Authority";
            excelHelper.MergeCells(worksheet, 26, 4, 26, 5, false);
            worksheet.Cells["D26:E26"].Value = "32 - Local Toll Authority";
            excelHelper.MergeCells(worksheet, 27, 4, 27, 5, false);
            worksheet.Cells["D27:E27"].Value = "60 - Other Federal Agencies";
            excelHelper.MergeCells(worksheet, 28, 4, 28, 5, false);
            worksheet.Cells["D28:E28"].Value = "62 - Bureau of Indian Affairs";
            excelHelper.MergeCells(worksheet, 29, 4, 29, 5, false);
            worksheet.Cells["D29:E29"].Value = "64 - U.S. Forest Service";
            excelHelper.MergeCells(worksheet, 30, 4, 30, 5, false);
            worksheet.Cells["D30:E30"].Value = "66 - National Park Service";
            excelHelper.MergeCells(worksheet, 31, 4, 31, 5, false);
            worksheet.Cells["D31:E31"].Value = "68 - Bureau of Land Management";
            excelHelper.MergeCells(worksheet, 32, 4, 32, 5, false);
            worksheet.Cells["D32:E32"].Value = "69 - Bureau of Reclamation";
            excelHelper.MergeCells(worksheet, 33, 4, 33, 5, false);
            worksheet.Cells["D33:E33"].Value = "70 - Military Reservation Corps Engineers";
            excelHelper.MergeCells(worksheet, 34, 4, 34, 5, false);
            worksheet.Cells["D34:E34"].Value = "80 - Unknown";
            excelHelper.MergeCells(worksheet, 35, 4, 35, 5, false);
            worksheet.Cells["D35:E35"].Value = "XX - Demolished/Replaced";

            flag = worksheet.DataValidations.AddListValidation("F15");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("F16");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F17");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F18");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F19");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F20");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F21");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F22");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F23");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F24");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F25");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F26");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F27");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F28");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F29");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F30");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F31");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F32");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F33");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F34");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            flag = worksheet.DataValidations.AddListValidation("F35");
            flag.Formula.Values.Add("N");
            flag.Formula.Values.Add("Y");
            worksheet.Cells["F15"].Value = "Y";
            worksheet.Cells["F16"].Value = "N";
            worksheet.Cells["F17"].Value = "N";
            worksheet.Cells["F18"].Value = "N";
            worksheet.Cells["F19"].Value = "N";
            worksheet.Cells["F20"].Value = "N";
            worksheet.Cells["F21"].Value = "N";
            worksheet.Cells["F22"].Value = "N";
            worksheet.Cells["F23"].Value = "N";
            worksheet.Cells["F24"].Value = "N";
            worksheet.Cells["F25"].Value = "N";
            worksheet.Cells["F26"].Value = "N";
            worksheet.Cells["F27"].Value = "N";
            worksheet.Cells["F28"].Value = "N";
            worksheet.Cells["F29"].Value = "N";
            worksheet.Cells["F30"].Value = "N";
            worksheet.Cells["F31"].Value = "N";
            worksheet.Cells["F32"].Value = "N";
            worksheet.Cells["F33"].Value = "N";
            worksheet.Cells["F34"].Value = "N";
            worksheet.Cells["F35"].Value = "N";
            excelHelper.ApplyBorder(worksheet.Cells[14, 4, 35, 6]);

            excelHelper.MergeCells(worksheet, 14, 8, 14, 10);
            worksheet.Cells["H14:J14"].Value = "5C22 Functional Class";
            excelHelper.MergeCells(worksheet, 15, 8, 15, 10);
            worksheet.Cells["H15:J15"].Value = "Rural";

            excelHelper.MergeCells(worksheet, 16, 8, 16, 9, false);
            worksheet.Cells["H16:I16"].Value = "01 - Principal Arterial - Interstate";
            excelHelper.MergeCells(worksheet, 17, 8, 17, 9, false);
            worksheet.Cells["H17:I17"].Value = "02 - Principal Arterial - Other";
            excelHelper.MergeCells(worksheet, 18, 8, 18, 9, false);
            worksheet.Cells["H18:I18"].Value = "06 - Minor Arterial";
            excelHelper.MergeCells(worksheet, 19, 8, 19, 9, false);
            worksheet.Cells["H19:I19"].Value = "07 - Major Collector";
            excelHelper.MergeCells(worksheet, 20, 8, 20, 9, false);
            worksheet.Cells["H20:I20"].Value = "08 - Minor Collector";
            excelHelper.MergeCells(worksheet, 21, 8, 21, 9, false);
            worksheet.Cells["H21:I21"].Value = "09 - Local";
            excelHelper.MergeCells(worksheet, 22, 8, 22, 9, false);
            worksheet.Cells["H22:I22"].Value = "NN - Other";

            flag = worksheet.DataValidations.AddListValidation("J16");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J17");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J18");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J19");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J20");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J21");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J22");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["J16"].Value = "Y";
            worksheet.Cells["J17"].Value = "Y";
            worksheet.Cells["J18"].Value = "Y";
            worksheet.Cells["J19"].Value = "Y";
            worksheet.Cells["J20"].Value = "Y";
            worksheet.Cells["J21"].Value = "Y";
            worksheet.Cells["J22"].Value = "Y";

            excelHelper.MergeCells(worksheet, 23, 8, 23, 10);
            worksheet.Cells["H23:J23"].Value = "Urban";

            excelHelper.MergeCells(worksheet, 24, 8, 24, 9, false);
            worksheet.Cells["H24:J24"].Value = "11 - Principal Arterial - Interstate";
            excelHelper.MergeCells(worksheet, 25, 8, 25, 9, false);
            worksheet.Cells["H25:I25"].Value = "12 - Principal Arterial - Other Freeway & Expressways";
            excelHelper.MergeCells(worksheet, 26, 8, 26, 9, false);
            worksheet.Cells["H26:I26"].Value = "14 - Other Principal Arterial";
            excelHelper.MergeCells(worksheet, 27, 8, 27, 9, false);
            worksheet.Cells["H27:I27"].Value = "16 - Minor Arterial";
            excelHelper.MergeCells(worksheet, 28, 8, 28, 9, false);
            worksheet.Cells["H28:I28"].Value = "17 - Collector";
            excelHelper.MergeCells(worksheet, 29, 8, 29, 9, false);
            worksheet.Cells["H29:I29"].Value = "19 - Local";
            excelHelper.MergeCells(worksheet, 30, 8, 30, 9, false);
            worksheet.Cells["H30:I30"].Value = "NN - Other";
            excelHelper.MergeCells(worksheet, 31, 8, 31, 9, false);
            worksheet.Cells["H31:I31"].Value = "99 - Ramp";

            flag = worksheet.DataValidations.AddListValidation("J24");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J25");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J26");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J27");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J28");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J29");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J30");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            flag = worksheet.DataValidations.AddListValidation("J31");
            flag.Formula.Values.Add("Y");
            flag.Formula.Values.Add("N");
            worksheet.Cells["J24"].Value = "Y";
            worksheet.Cells["J25"].Value = "Y";
            worksheet.Cells["J26"].Value = "Y";
            worksheet.Cells["J27"].Value = "Y";
            worksheet.Cells["J28"].Value = "Y";
            worksheet.Cells["J29"].Value = "Y";
            worksheet.Cells["J30"].Value = "Y";
            worksheet.Cells["J31"].Value = "Y";
            excelHelper.ApplyBorder(worksheet.Cells[14, 8, 31, 10]);
        }

        private void FillSimulationDetails(ExcelWorksheet worksheet, SimulationAnalysisModel investmentPeriod, double? inflationRate)
        {
            excelHelper.MergeCells(worksheet, 6, 6, 6, 8);
            excelHelper.MergeCells(worksheet, 8, 6, 8, 7);
            excelHelper.MergeCells(worksheet, 10, 6, 10, 7);
            excelHelper.MergeCells(worksheet, 12, 6, 12, 7);

            worksheet.Cells["F6:H6"].Value = "Investment:";
            worksheet.Cells["F8:G8"].Value = "Start Year:";
            worksheet.Cells["F10:G10"].Value = "Analysis Period:";
            worksheet.Cells["F12:G12"].Value = "Inflation Rate:";

            excelHelper.ApplyBorder(worksheet.Cells[6, 6, 12, 8]);

            worksheet.Cells["H8"].Value = investmentPeriod.StartYear;
            worksheet.Cells["H10"].Value = investmentPeriod.AnalysisPeriod;
            worksheet.Cells["H12"].Value = inflationRate;

            excelHelper.ApplyBorder(worksheet.Cells[8, 8, 12, 8]);
        }

        private void FillAnalysisDetails(ExcelWorksheet worksheet, SimulationAnalysisModel investmentPeriod)
        {
            excelHelper.MergeCells(worksheet, 6, 12, 6, 15);
            excelHelper.MergeCells(worksheet, 8, 12, 8, 13);
            excelHelper.MergeCells(worksheet, 10, 12, 10, 13);
            excelHelper.MergeCells(worksheet, 12, 12, 12, 13);
            excelHelper.MergeCells(worksheet, 14, 12, 14, 13);

            excelHelper.ApplyBorder(worksheet.Cells[6, 12, 14, 15]);

            excelHelper.MergeCells(worksheet, 8, 14, 8, 15);
            excelHelper.MergeCells(worksheet, 10, 14, 10, 15, false);
            excelHelper.MergeCells(worksheet, 12, 14, 12, 15, false);
            excelHelper.MergeCells(worksheet, 14, 14, 14, 15, false);

            excelHelper.ApplyBorder(worksheet.Cells[8, 14, 14, 15]);

            worksheet.Cells["L6:O6"].Value = "Analysis:";
            worksheet.Cells["L8:M8"].Value = "Optimization:";
            worksheet.Cells["L10:M10"].Value = "Budget:";
            worksheet.Cells["L12:M12"].Value = "Weighting:";
            worksheet.Cells["L14:M14"].Value = "Benefit:";
            var optimization = worksheet.DataValidations.AddListValidation("N8:O8");
            optimization.Formula.Values.Add("Incremental Benefit/Cost");
            optimization.Formula.Values.Add("Maximum Benefit");
            optimization.Formula.Values.Add("Remaining Life/Cost");
            optimization.Formula.Values.Add("Maximum Remaining Life");
            optimization.Formula.Values.Add("Multi-year Incremental Benefit/Cost");
            optimization.Formula.Values.Add("Multi-year Maximum Benefit");
            optimization.Formula.Values.Add("Multi-year Remaining Life/Cost");
            optimization.Formula.Values.Add("Multi-year Maximum Life");
            optimization.AllowBlank = false;
            worksheet.Cells["N8:O8"].Value = investmentPeriod.OptimizationType;

            var budgets = worksheet.DataValidations.AddListValidation("N10:O10");
            budgets.Formula.Values.Add("No Spending");
            budgets.Formula.Values.Add("As Budget Permits");
            budgets.Formula.Values.Add("Until Targets Met");
            budgets.Formula.Values.Add("Until Deficient Met");
            budgets.Formula.Values.Add("Targets/Deficient Met");
            budgets.Formula.Values.Add("Unlimited");
            worksheet.Cells["N10:O10"].Value = investmentPeriod.BudgetType;
            worksheet.Cells["N12:O12"].Value = investmentPeriod.WeightingAttribute;
            worksheet.Cells["N14:O14"].Value = investmentPeriod.BenefitAttribute;
        }

        private void FillPriorities(ExcelWorksheet worksheet, List<PriorityModel> priorities)
        {
            excelHelper.MergeCells(worksheet, 19, 12, 19, worksheet.Dimension.End.Column);
            excelHelper.MergeCells(worksheet, 20, 13, 20, worksheet.Dimension.End.Column);

            excelHelper.ApplyBorder(worksheet.Cells[20, 12, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column]);

            worksheet.Cells["L19:Z19"].Value = "Analysis Priorites:";

            worksheet.Cells["L20"].Value = "Number";
            worksheet.Cells["M20"].Value = "Criteria:";

            var startingRow = 21;
            foreach (var item in priorities)
            {
                excelHelper.MergeCells(worksheet, startingRow, 13, startingRow, worksheet.Dimension.End.Column, false);
                excelHelper.ApplyBorder(worksheet.Cells[21, 12, worksheet.Dimension.End.Row, worksheet.Dimension.End.Column]);
                worksheet.Cells[startingRow, 12].Value = startingRow - 20;
                worksheet.Cells[startingRow, 13].Value = item.Criteria;
                startingRow++;
            }
        }

        private void FillJurisdictionCriteria(ExcelWorksheet worksheet, string criteria)
        {
            excelHelper.MergeCells(worksheet, 16, 12, 17, 13);
            excelHelper.MergeCells(worksheet, 16, 14, 17, 26, false);

            excelHelper.ApplyBorder(worksheet.Cells[16, 14, 17, 26]);

            worksheet.Cells["L16:M16"].Value = "Jurisdiction Criteria:";
            worksheet.Cells["N16:Z16"].Value = criteria;
        }

        private void FillInvestmentAndBudgetCriteria(ExcelWorksheet worksheet, InvestmentLibraryModel inflationAndInvestments, List<Models.CriteriaDrivenBudgets.CriteriaDrivenBudgetsModel> criterias)
        {
            var currencyFormat = "_-$* #,##0.00_-;-$* #,##0.00_-;_-$* \"-\"??_-;_-@_-";
            worksheet.Cells[38, 1].Value = "Years";
            worksheet.Cells[38, 2].Value = "Total Funding";

            var startingRowInvestment = 40;
            var startingBudgetHeaderColumn = 2;
            var nextBudget = 0;
            var investmentGrid = new Dictionary<int, List<(string BudgetName, double? BudgetAmount)>>();
            foreach (var item in inflationAndInvestments.BudgetYears)
            {
                if (!investmentGrid.ContainsKey(item.Year))
                {
                    investmentGrid.Add(item.Year, new List<(string BudgetName, double? BudgetAmount)> { (item.BudgetName, item.BudgetAmount) });
                }
                else
                {
                    investmentGrid[item.Year].Add((item.BudgetName, item.BudgetAmount));
                }
            }

            var firstRow = true;
            foreach (var item in investmentGrid)
            {
                worksheet.Cells[startingRowInvestment, 1].Value = item.Key;
                foreach (var budget in item.Value)
                {
                    if (firstRow == true)
                    {
                        worksheet.Cells[39, startingBudgetHeaderColumn + nextBudget].Value = budget.BudgetName;
                        worksheet.Cells[startingRowInvestment, startingBudgetHeaderColumn + nextBudget].Value = budget.BudgetAmount.Value;
                        worksheet.Cells[startingRowInvestment, startingBudgetHeaderColumn + nextBudget].Style.Numberformat.Format = currencyFormat;
                        nextBudget++;
                        continue;
                    }
                    for (var column = startingBudgetHeaderColumn; column <= item.Value.Count + 1; column++)
                    {
                        if (worksheet.Cells[39, column].Value.ToString() == budget.BudgetName)
                        {
                            worksheet.Cells[startingRowInvestment, column].Style.Numberformat.Format = currencyFormat;
                            worksheet.Cells[startingRowInvestment, column].Value = budget.BudgetAmount.Value;
                            break;
                        }
                    }
                }
                startingRowInvestment++;
                firstRow = false;
                nextBudget = 0;
            }
            excelHelper.MergeCells(worksheet, 38, 1, 39, 1);
            excelHelper.MergeCells(worksheet, 38, 2, 38, inflationAndInvestments.BudgetOrder.Count + 2);
            excelHelper.ApplyBorder(worksheet.Cells[38, 1, startingRowInvestment - 1, inflationAndInvestments.BudgetOrder.Count + 3]);
            FillBudgetCriteria(worksheet, startingRowInvestment, criterias);
        }

        private void FillBudgetCriteria(ExcelWorksheet worksheet, int startingRowInvestment, List<CriteriaDrivenBudgetsModel> criterias)
        {
            var rowToApplyBorder = startingRowInvestment + 2;
            worksheet.Cells[startingRowInvestment + 2, 1].Value = "Budget Criteria";
            excelHelper.MergeCells(worksheet, startingRowInvestment + 2, 1, startingRowInvestment + 2, 5);

            worksheet.Cells[startingRowInvestment + 3, 1].Value = "Budget Name";
            worksheet.Cells[startingRowInvestment + 3, 2].Value = "Criteria";
            var cells = worksheet.Cells[startingRowInvestment + 3, 1, startingRowInvestment + 3, 2];
            excelHelper.ApplyStyle(cells);
            foreach (var item in criterias)
            {
                worksheet.Cells[startingRowInvestment + 4, 1].Value = item.BudgetName;
                worksheet.Cells[startingRowInvestment + 4, 2].Value = item.Criteria;
                excelHelper.MergeCells(worksheet, startingRowInvestment + 4, 2, startingRowInvestment + 4, 5, false);
                startingRowInvestment++;
            }
            excelHelper.ApplyBorder(worksheet.Cells[rowToApplyBorder, 1, startingRowInvestment + 3, 5]);
        }
    }
}

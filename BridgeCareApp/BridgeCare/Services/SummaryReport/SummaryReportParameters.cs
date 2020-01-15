using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BridgeCare.Services.SummaryReport
{
    public class SummaryReportParameters
    {
        private readonly BridgeCareContext db;
        private readonly ISimulationAnalysis analysisData;
        private readonly IInvestmentLibrary getInflationRate;
        private readonly ExcelHelper excelHelper;

        public SummaryReportParameters(ISimulationAnalysis simulationAnalysis, IInvestmentLibrary inflationRate, ExcelHelper excelHelper, BridgeCareContext db)
        {
            analysisData = simulationAnalysis ?? throw new ArgumentNullException(nameof(simulationAnalysis));
            getInflationRate = inflationRate ?? throw new ArgumentNullException(nameof(inflationRate));
            this.excelHelper = excelHelper;
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        internal void Fill(ExcelWorksheet worksheet, SimulationModel simulationModel, List<int> simulationYears)
        {
            var investmentPeriod = analysisData.GetSimulationAnalysis(simulationModel.SimulationId, db);
            var selectedInflationRate = getInflationRate.GetSimulationInvestmentLibrary(simulationModel.SimulationId, db);

            // Simulation Name format
            excelHelper.MergeCells(worksheet, 1, 1, 1, 2);
            excelHelper.MergeCells(worksheet, 1, 3, 1, 10);

            worksheet.Cells["A1:B1"].Value = "Simulation Name";
            worksheet.Cells["C1:J1"].Value = simulationModel.SimulationName;

            excelHelper.ApplyBorder(worksheet.Cells[1, 1, 1, 2]);
            excelHelper.ApplyBorder(worksheet.Cells[1, 3, 1, 10]);

            // End of Simulation Name format

            // Investment format
            excelHelper.MergeCells(worksheet, 6, 6, 6, 8);
            excelHelper.MergeCells(worksheet, 8, 6, 8, 7);
            excelHelper.MergeCells(worksheet, 10, 6, 10, 7);
            excelHelper.MergeCells(worksheet, 12, 6, 12, 7);

            worksheet.Cells["F6:H6"].Value = "Investment:";
            worksheet.Cells["F8:G8"].Value = "Start Year:";
            worksheet.Cells["F10:G10"].Value = "Analysis Period:";
            worksheet.Cells["F12:G12"].Value = "Inflation Rate:";

            excelHelper.ApplyBorder(worksheet.Cells[6, 6, 6, 8]);
            excelHelper.ApplyBorder(worksheet.Cells[8, 6, 8, 7]);
            excelHelper.ApplyBorder(worksheet.Cells[10, 6, 10, 7]);
            excelHelper.ApplyBorder(worksheet.Cells[12, 6, 12, 7]);

            worksheet.Cells["H8"].Value = investmentPeriod.StartYear;
            worksheet.Cells["H10"].Value = investmentPeriod.AnalysisPeriod;
            worksheet.Cells["H12"].Value = selectedInflationRate.InflationRate;

            excelHelper.ApplyBorder(worksheet.Cells[8, 8]);
            excelHelper.ApplyBorder(worksheet.Cells[10, 8]);
            excelHelper.ApplyBorder(worksheet.Cells[12, 8]);
            // End of Investment format

            //Analysis format
            excelHelper.MergeCells(worksheet, 6, 12, 6, 15);
            excelHelper.MergeCells(worksheet, 8, 12, 8, 13);
            excelHelper.MergeCells(worksheet, 10, 12, 10, 13);
            excelHelper.MergeCells(worksheet, 12, 12, 12, 13);
            excelHelper.MergeCells(worksheet, 14, 12, 14, 13);

            excelHelper.ApplyBorder(worksheet.Cells[6, 12, 6, 15]);

            excelHelper.ApplyBorder(worksheet.Cells[8, 12, 8, 13]);
            excelHelper.ApplyBorder(worksheet.Cells[10, 12, 10, 13]);
            excelHelper.ApplyBorder(worksheet.Cells[12, 12, 12, 13]);
            excelHelper.ApplyBorder(worksheet.Cells[14, 12, 14, 13]);

            excelHelper.MergeCells(worksheet, 8, 14, 8, 15);
            excelHelper.MergeCells(worksheet, 10, 14, 10, 15);
            excelHelper.MergeCells(worksheet, 12, 14, 12, 15);
            excelHelper.MergeCells(worksheet, 14, 14, 14, 15);

            excelHelper.ApplyBorder(worksheet.Cells[8, 14, 8, 15]);
            excelHelper.ApplyBorder(worksheet.Cells[10, 14, 10, 15]);
            excelHelper.ApplyBorder(worksheet.Cells[12, 14, 12, 15]);
            excelHelper.ApplyBorder(worksheet.Cells[14, 14, 14, 15]);

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
            //End of Analysis format
        }
    }
}

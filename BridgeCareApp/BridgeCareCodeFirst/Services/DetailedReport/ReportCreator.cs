using BridgeCare.Interfaces;
using BridgeCare.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace BridgeCare.Services
{
    public class ReportCreator : IReportCreator
    {
        private readonly BridgeCareContext db;
        private readonly IDetailedReport investment;
        private readonly TargetReport targetReport;
        private readonly DeficientReport deficientReport;
        private readonly FillDetailedReport detailReport;
        private readonly Budget budgetReport;

        public ReportCreator(IDetailedReport yearlyReport, Budget getBudget,
                   TargetReport targetCell, DeficientReport deficientCell, FillDetailedReport details, BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            investment = yearlyReport ?? throw new ArgumentNullException(nameof(yearlyReport));
            targetReport = targetCell ?? throw new ArgumentNullException(nameof(targetCell));
            deficientReport = deficientCell ?? throw new ArgumentNullException(nameof(deficientCell));
            detailReport = details ?? throw new ArgumentNullException(nameof(details));
            budgetReport = getBudget ?? throw new ArgumentNullException(nameof(getBudget));
        }

        public byte[] CreateExcelReport(SimulationModel data)
        {
            // Getting data from the database
            var yearlyInvestment = investment.GetYearsData(data);
            // Using BridgeCareContext because manual control is needed for the creation of the object.
            // This object is going through data heavy operation. That is why it is not shared.
            var dbContext = new BridgeCareContext();
            var totalYears = yearlyInvestment.Select(_ => _.Year).Distinct().ToArray();

            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("DetailedReport.xlsx")))
            {
                //create a WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Detailed report");

                detailReport.Fill(worksheet, totalYears, data, dbContext);

                // Adding new Excel TAB for Budget results
                ExcelWorksheet budgetSheet = excelPackage.Workbook.Worksheets.Add("Budget results");

                budgetReport.Fill(budgetSheet, totalYears, data, yearlyInvestment);

                // Adding new Excel TAB for Deficient results
                ExcelWorksheet deficientSheet = excelPackage.Workbook.Worksheets.Add("Deficient results");
                deficientReport.Fill(deficientSheet, data, totalYears);

                // Adding new Excel TAB for Target Results
                ExcelWorksheet targetSheet = excelPackage.Workbook.Worksheets.Add("Target results");
                targetReport.Fill(targetSheet, data, totalYears);
                return excelPackage.GetAsByteArray();
            }
        }
    }
}
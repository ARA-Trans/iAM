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
    public class DetailedReportRepository : IReportCreater
    {
        private readonly BridgeCareContext db;
        private readonly IDetailedReport detailedReport;
        private readonly FillTargets fillTargets;
        private readonly FillDeficients fillDeficients;
        private readonly FillDetailedReport fillDetailedReport;
        private readonly FillBudget fillBudget;

        public DetailedReportRepository(IDetailedReport yearlyReport, FillBudget getBudget,
                   FillTargets targetCell, FillDeficients deficientCell, FillDetailedReport details, BridgeCareContext context)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            detailedReport = yearlyReport ?? throw new ArgumentNullException(nameof(yearlyReport));
            fillTargets = targetCell ?? throw new ArgumentNullException(nameof(targetCell));
            fillDeficients = deficientCell ?? throw new ArgumentNullException(nameof(deficientCell));
            fillDetailedReport = details ?? throw new ArgumentNullException(nameof(details));
            fillBudget = getBudget ?? throw new ArgumentNullException(nameof(getBudget));
        }

        public byte[] CreateExcelReport(SimulationResult data)
        {
            // Getting data from the database
            var yearlyInvestment = detailedReport.GetYearsData(data);
            // Using BridgeCareContext because manual control is needed for the creation of the object.
            // This object is going through data heavy operation. That is why it is not shared.
            var dbContext = new BridgeCareContext();
            var totalYears = yearlyInvestment.Select(_ => _.Year).Distinct().ToArray();

            using (ExcelPackage excelPackage = new ExcelPackage(new System.IO.FileInfo("DetailedReport.xlsx")))
            {
                //create a WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Detailed report");

                fillDetailedReport.FillYearlyData(worksheet, totalYears, data, dbContext);

                // Adding new Excel TAB for Budget results
                ExcelWorksheet budgetReport = excelPackage.Workbook.Worksheets.Add("Budget results");

                fillBudget.FillTotalView(budgetReport, totalYears, data, yearlyInvestment);

                // Adding new Excel TAB for Deficient results
                ExcelWorksheet deficientReport = excelPackage.Workbook.Worksheets.Add("Deficient results");
                fillDeficients.FillDeficient(deficientReport, data, totalYears);

                // Adding new Excel TAB for Target Results
                ExcelWorksheet targetReport = excelPackage.Workbook.Worksheets.Add("Target results");
                fillTargets.FillTarget(targetReport, data, totalYears);
                return excelPackage.GetAsByteArray();
            }
        }
    }
}